using DotNet.Testcontainers.Builders;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NUnit.Framework;
using Respawn;
using Testcontainers.PostgreSql;

[SetUpFixture]
public class Tests
{
    private static readonly PostgreSqlContainer DatabaseContainer = new PostgreSqlBuilder()
      .WithImage("postgres:latest")
      .WithDatabase("testdb")
      .WithUsername("testuser")
      .WithPassword("testpassword")
      .WithWaitStrategy(Wait
        .ForUnixContainer()
        .UntilMessageIsLogged("database system is ready to accept connections"))
      .Build();

    private static Respawner respawner = default!;

    private static RespawnerOptions respawnerOptions = new() { DbAdapter = DbAdapter.Postgres };

    public static string ConnectionString => DatabaseContainer.GetConnectionString();

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        await DatabaseContainer.StartAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
          .UseNpgsql(ConnectionString)
          .Options;

        using var db = new AppDbContext(options);

        await db.Database.EnsureCreatedAsync();

        await using var connection = new NpgsqlConnection(ConnectionString);

        await connection.OpenAsync();

        respawner = await Respawner.CreateAsync(connection, respawnerOptions);
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await DatabaseContainer.StopAsync();
        await DatabaseContainer.DisposeAsync();
    }

    public static async Task ResetDatabase()
    {
        await using var connection = new NpgsqlConnection(ConnectionString);

        await connection.OpenAsync();

        await respawner.ResetAsync(connection);
    }
}