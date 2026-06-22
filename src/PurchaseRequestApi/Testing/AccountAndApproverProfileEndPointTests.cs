using Infrastructure.Database;
using Infrastructure.Database.Entities;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Testing;

namespace Testing.EndpointTests
{
    // ═══════════════════════════════════════════════════════
    //  ACCOUNT
    // ═══════════════════════════════════════════════════════

    [TestFixture]
    public class AccountEndpointTests : EndpointTestBase
    {
        // POST /Account/login ─────────────────────────────

        [Test]
        public async Task Login_ValidCredentials_Returns200WithToken()
        {
            var region = await SeedRegionAsync();
            var role   = await SeedRoleAsync();
            await SeedAccountAsync(region, new List<Role> { role }, "login.user", "secret");

            var response = await Client.PostAsJsonAsync("/Account/login", new { Login = "login.user", Password = "secret" });

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            Assert.That(json.GetProperty("data").GetProperty("token").GetString(), Is.Not.Empty);
        }

        [Test]
        public async Task Login_WrongPassword_Returns401()
        {
            var region = await SeedRegionAsync();
            var role   = await SeedRoleAsync();
            await SeedAccountAsync(region, new List<Role> { role }, "login.user2", "correct");

            var response = await Client.PostAsJsonAsync("/Account/login", new { Login = "login.user2", Password = "wrong" });

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task Login_NonExistentUser_Returns401()
        {
            var response = await Client.PostAsJsonAsync("/Account/login", new { Login = "ghost", Password = "x" });

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        // POST /Account ───────────────────────────────────

        [Test]
        public async Task Create_ValidDto_Returns200()
        {
            var region = await SeedRegionAsync();
            var role   = await SeedRoleAsync();

            var dto = new
            {
                Login = "new.user",
                Password = "pw",
                Name = "New User",
                RegionId = region.Id,
                RoleIds = new[] { role.Id }
            };

            var response = await Client.PostAsJsonAsync("/Account", dto);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Create_ValidDto_PersistsAccount()
        {
            var region = await SeedRegionAsync();
            var role   = await SeedRoleAsync();

            await Client.PostAsJsonAsync("/Account", new
            {
                Login = "persisted.user",
                Password = "pw",
                Name = "Persisted",
                RegionId = region.Id,
                RoleIds = new[] { role.Id }
            });

            await using var db = CreateDbContext();
            Assert.That(await db.Accounts.AnyAsync(a => a.Login == "persisted.user"), Is.True);
        }

        // GET /Account ────────────────────────────────────

        [Test]
        public async Task GetAll_WithoutToken_Returns401()
        {
            var response = await Client.GetAsync("/Account");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task GetAll_AsAdmin_Returns200()
        {
            await AuthAsAdminAsync();
            var response = await Client.GetAsync("/Account");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task GetAll_AsRequester_Returns403()
        {
            await AuthAsRequesterAsync();
            var response = await Client.GetAsync("/Account");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        // DELETE /Account/{id} ────────────────────────────

        [Test]
        public async Task Delete_AsAdmin_ExistingAccount_Returns200()
        {
            await AuthAsAdminAsync();
            var region  = await SeedRegionAsync(name: "Del Region");
            var role    = await SeedRoleAsync("Requester");
            var account = await SeedAccountAsync(region, new List<Role> { role }, "to.delete", "pw");

            var response = await Client.DeleteAsync($"/Account/{account.Id}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Delete_AsAdmin_NonExistentId_Returns400()
        {
            await AuthAsAdminAsync();
            var response = await Client.DeleteAsync($"/Account/{Guid.NewGuid()}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task Delete_WithoutToken_Returns401()
        {
            var response = await Client.DeleteAsync($"/Account/{Guid.NewGuid()}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }
    }

    // ═══════════════════════════════════════════════════════
    //  APPROVER PROFILE
    // ═══════════════════════════════════════════════════════

    [TestFixture]
    public class ApproverProfileEndpointTests : EndpointTestBase
    {
        // GET /ApproverProfile ────────────────────────────

        [Test]
        public async Task GetAll_NoAuth_Returns200()
        {
            // No [Authorize] on GetAll
            var response = await Client.GetAsync("/ApproverProfile");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        // POST /ApproverProfile ───────────────────────────

        [Test]
        public async Task Create_AsAdmin_Returns200()
        {
            await AuthAsAdminAsync();
            var response = await Client.PostAsJsonAsync("/ApproverProfile",
                new { Name = "Junior", MinAmount = 0m, MaxAmount = 500m });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Create_WithoutToken_Returns401()
        {
            var response = await Client.PostAsJsonAsync("/ApproverProfile",
                new { Name = "Junior", MinAmount = 0m, MaxAmount = 500m });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task Create_AsRequester_Returns403()
        {
            await AuthAsRequesterAsync();
            var response = await Client.PostAsJsonAsync("/ApproverProfile",
                new { Name = "Junior", MinAmount = 0m, MaxAmount = 500m });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        // PUT /ApproverProfile/{id} ───────────────────────

        [Test]
        public async Task Update_AsAdmin_ExistingProfile_Returns200()
        {
            await AuthAsAdminAsync();

            var createRes = await Client.PostAsJsonAsync("/ApproverProfile",
                new { Name = "To Update", MinAmount = 0m, MaxAmount = 100m });
            var created = await createRes.Content.ReadFromJsonAsync<JsonElement>();
            var id = created.GetProperty("data").GetProperty("id").GetGuid();

            var response = await Client.PutAsJsonAsync($"/ApproverProfile/{id}",
                new { Name = "Updated", MinAmount = 50m, MaxAmount = 200m });

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Update_AsAdmin_NonExistentId_Returns400()
        {
            await AuthAsAdminAsync();
            var response = await Client.PutAsJsonAsync($"/ApproverProfile/{Guid.NewGuid()}",
                new { Name = "X", MinAmount = 0m, MaxAmount = 1m });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        // DELETE /ApproverProfile/{id} ────────────────────

        [Test]
        public async Task Delete_AsAdmin_ExistingProfile_Returns200()
        {
            await AuthAsAdminAsync();

            var createRes = await Client.PostAsJsonAsync("/ApproverProfile",
                new { Name = "To Delete", MinAmount = 0m, MaxAmount = 100m });
            var created = await createRes.Content.ReadFromJsonAsync<JsonElement>();
            var id = created.GetProperty("data").GetProperty("id").GetGuid();

            var response = await Client.DeleteAsync($"/ApproverProfile/{id}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Delete_WithoutToken_Returns401()
        {
            var response = await Client.DeleteAsync($"/ApproverProfile/{Guid.NewGuid()}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }
    }
}