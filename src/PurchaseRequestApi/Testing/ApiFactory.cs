using Infrastructure.Database;
using Infrastructure.Database.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using NUnit.Framework;
using Respawn;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Testcontainers.PostgreSql;

// All endpoint test fixtures share one Testcontainer + Respawn reset cycle.
// Force sequential execution across the whole assembly so fixtures never
// race on starting/stopping the container or stomp each other's DB state.
// (NonParallelizableAttribute has no assembly-level effect in NUnit; the
// correct mechanism is LevelOfParallelism combined with NonParallelizable
// on the base fixture class, which IS inherited by derived [TestFixture]s.)
[assembly: NUnit.Framework.LevelOfParallelism(1)]

namespace Testing
{
    /// <summary>
    /// Shared Testcontainer + Respawn lifecycle for all endpoint tests.
    /// One container is started per test run; Respawn resets between tests.
    /// </summary>
    public static class ApiTestDb
    {
        private static PostgreSqlContainer? _container;
        private static Respawner? _respawner;

        public static string ConnectionString => _container!.GetConnectionString();

        // Discrete pieces matching Infrastructure.ConfigureServices.AddDb's
        // DbOptions shape (Host/Database/Username/Password). PostgreSqlBuilder
        // fixes Database/Username/Password to what we configure below; Host
        // must come from the container's mapped port since Testcontainers
        // assigns a random host port.
        public const string Database = "apitestdb";
        public const string Username = "testuser";
        public const string Password = "testpassword";
        public static string Host => $"{_container!.Hostname}:{_container.GetMappedPublicPort(5432)}";

        public static async Task InitAsync()
        {
            _container = new PostgreSqlBuilder()
                .WithImage("postgres:latest")
                .WithDatabase(Database)
                .WithUsername(Username)
                .WithPassword(Password)
                .Build();

            await _container.StartAsync();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(ConnectionString)
                .Options;

            using var db = new AppDbContext(options);
            await db.Database.EnsureCreatedAsync();

            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();
            _respawner = await Respawner.CreateAsync(conn, new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = new[] { "public" }
            });
        }

        public static async Task ResetAsync()
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();
            await _respawner!.ResetAsync(conn);

            // Diagnostic: confirm the reset actually cleared Accounts.
            await using var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM \"Accounts\"", conn);
            var remaining = (long)(await cmd.ExecuteScalarAsync())!;
            if (remaining != 0)
            {
                throw new Exception(
                    $"DIAGNOSTIC: Respawn.ResetAsync completed but {remaining} row(s) remain in \"Accounts\".");
            }
        }

        public static async Task DisposeAsync()
        {
            await _container!.StopAsync();
            await _container.DisposeAsync();
        }
    }

    /// <summary>
    /// WebApplicationFactory that overrides the DB connection string and JWT settings
    /// so tests use the Testcontainer database with a known signing key.
    /// </summary>
    public class ApiWebApplicationFactory : WebApplicationFactory<Program>
    {
        public const string TestJwtSecret = "super-secret-key-for-testing-only-32chars!!";
        public const string TestJwtIssuer = "TestIssuer";
        public const string TestJwtAudience = "TestAudience";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["DbOptions:Host"]              = ApiTestDb.Host,
                    ["DbOptions:Database"]          = ApiTestDb.Database,
                    ["DbOptions:Username"]          = ApiTestDb.Username,
                    ["DbOptions:Password"]          = ApiTestDb.Password,

                    ["JwtOptions:Secret"]          = TestJwtSecret,
                    ["JwtOptions:Issuer"]          = TestJwtIssuer,
                    ["JwtOptions:Audience"]        = TestJwtAudience,
                    ["JwtOptions:ExpirationHours"] = "1",
                });
            });

            builder.ConfigureServices(services =>
            {
                // AddDb() in Infrastructure resolves DbOptions eagerly via
                // services.BuildServiceProvider() at the moment Program.cs calls
                // it — BEFORE WebApplicationFactory's ConfigureAppConfiguration
                // override above has been layered into builder.Configuration.
                // That means the connection string baked into AppDbContext's
                // UseNpgsql(...) closure is captured from the REAL appsettings.json
                // (whatever dev DB is configured there), not the Testcontainer,
                // no matter what config keys we override here.
                //
                // Fix: remove whatever AppDbContext registration AddDb() already
                // added and re-register it pointing explicitly at the
                // Testcontainer connection string, captured directly (not via
                // IOptions indirection), so there is no eager-resolution window
                // for stale config to leak through.
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<AppDbContext>(opt =>
                    opt.UseNpgsql(ApiTestDb.ConnectionString));

                // AddJwtAuth() has the identical eager-resolution bug as AddDb():
                // it reads JwtOptions via a throwaway BuildServiceProvider() call
                // at Program.cs execution time, baking the REAL appsettings.json
                // signing key/issuer/audience into JwtBearerOptions before our
                // config override above has a chance to apply. LoginHandler signs
                // tokens correctly (it resolves IOptions<JwtOptions> via normal
                // DI, which DOES see the override), but the bearer validation
                // handler would reject those tokens because it's still configured
                // with the wrong key/issuer/audience. Re-configure it explicitly.
                services.PostConfigure<Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerOptions>(
                    Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = TestJwtIssuer,
                            ValidAudience = TestJwtAudience,
                            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                                System.Text.Encoding.UTF8.GetBytes(TestJwtSecret))
                        };
                    });
            });
        }
    }

    /// <summary>
    /// Starts the Testcontainer ONCE for the entire assembly run and disposes it
    /// after all tests complete. Declared once here — NOT per fixture — because
    /// duplicate OneTimeSetUp/TearDown on every EndpointTestBase subclass caused
    /// concurrent fixtures to race on starting/stopping the same static container,
    /// producing spurious connection failures and login 401s.
    /// </summary>
    [SetUpFixture]
    public class GlobalApiTestSetup
    {
        [OneTimeSetUp]
        public async Task RunBeforeAnyTests() => await ApiTestDb.InitAsync();

        [OneTimeTearDown]
        public async Task RunAfterAllTests() => await ApiTestDb.DisposeAsync();
    }

    /// <summary>
    /// All endpoint test fixtures must run sequentially against the shared DB —
    /// Respawn resets between tests, so concurrent fixtures would corrupt each
    /// other's state. NUnit parallelizes fixtures by default; this opts out.
    /// </summary>
    [NonParallelizable]
    public abstract class EndpointTestBase
    {
        protected ApiWebApplicationFactory Factory = default!;
        protected HttpClient Client = default!;

        [SetUp]
        public void SetUp()
        {
            Factory = new ApiWebApplicationFactory();
            Client = Factory.CreateClient();
        }

        [TearDown]
        public async Task TearDown()
        {
            await ApiTestDb.ResetAsync();
            Client.Dispose();
            await Factory.DisposeAsync();
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        protected AppDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(ApiTestDb.ConnectionString)
                .Options;
            return new AppDbContext(options);
        }

        /// <summary>Seeds a Region directly into the DB and returns it.</summary>
        protected async Task<Region> SeedRegionAsync(string name = "Test Region", string currency = "USD")
        {
            await using var db = CreateDbContext();
            var region = new Region { Id = Guid.NewGuid(), Name = name, Currency = currency };
            db.Regions.Add(region);
            await db.SaveChangesAsync();
            return region;
        }

        /// <summary>Seeds a Role directly into the DB and returns it.</summary>
        protected async Task<Role> SeedRoleAsync(string name = "Requester")
        {
            await using var db = CreateDbContext();
            var role = new Role { Id = Guid.NewGuid(), Name = name };
            db.Roles.Add(role);
            await db.SaveChangesAsync();
            return role;
        }

        /// <summary>Seeds an Account with the given roles and returns (login, password).</summary>
        protected async Task<Account> SeedAccountAsync(
            Region region,
            List<Role> roles,
            string login = "test.user",
            string password = "password123")
        {
            await using var db = CreateDbContext();
            var rolesInDb = await db.Roles.Where(r => roles.Select(x => x.Id).Contains(r.Id)).ToListAsync();
            var account = new Account
            {
                Id = Guid.NewGuid(),
                Login = login,
                Password = password,
                Name = "Test User",
                RegionId = region.Id,
                Role = rolesInDb
            };
            db.Accounts.Add(account);
            await db.SaveChangesAsync();
            return account;
        }

        /// <summary>Logs in via POST /Account/login and sets the Bearer token on Client.</summary>
        protected async Task AuthenticateAsync(string login, string password)
        {
            var response = await Client.PostAsJsonAsync("/Account/login", new { Login = login, Password = password });

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new Exception(
                    $"Login failed for '{login}': {(int)response.StatusCode} {response.StatusCode}. Body: {body}");
            }

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            var token = json.GetProperty("data").GetProperty("token").GetString()!;
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        /// <summary>Seeds a requester account and authenticates. Returns the account.</summary>
        protected async Task<Account> AuthAsRequesterAsync()
        {
            var region = await SeedRegionAsync();
            var role   = await SeedRoleAsync("Requester");
            var account = await SeedAccountAsync(region, new List<Role> { role }, "requester", "pw");
            await AuthenticateAsync("requester", "pw");
            return account;
        }

        /// <summary>Seeds an admin account and authenticates. Returns the account.</summary>
        protected async Task<Account> AuthAsAdminAsync()
        {
            var region = await SeedRegionAsync(name: "Admin Region");
            var role   = await SeedRoleAsync("Admin");
            var account = await SeedAccountAsync(region, new List<Role> { role }, "admin.user", "adminpw");
            await AuthenticateAsync("admin.user", "adminpw");
            return account;
        }

        /// <summary>Seeds an approver account and authenticates. Returns the account.</summary>
        protected async Task<Account> AuthAsApproverAsync()
        {
            var region = await SeedRegionAsync(name: "Approver Region");
            var role   = await SeedRoleAsync("Approver");
            var account = await SeedAccountAsync(region, new List<Role> { role }, "approver.user", "approverpw");
            await AuthenticateAsync("approver.user", "approverpw");
            return account;
        }
    }
}