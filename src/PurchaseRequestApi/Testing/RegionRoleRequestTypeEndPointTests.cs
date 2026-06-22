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
    //  REGION
    // ═══════════════════════════════════════════════════════

    [TestFixture]
    public class RegionEndpointTests : EndpointTestBase
    {
        // GET /Region ─────────────────────────────────────

        [Test]
        public async Task GetAll_NoAuth_Returns200()
        {
            var response = await Client.GetAsync("/Region");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        // GET /Region/{id} ────────────────────────────────

        [Test]
        public async Task GetById_ExistingId_Returns200()
        {
            var region = await SeedRegionAsync("EU", "EUR");
            var response = await Client.GetAsync($"/Region/{region.Id}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task GetById_NonExistentId_Returns404()
        {
            var response = await Client.GetAsync($"/Region/{Guid.NewGuid()}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        // POST /Region ────────────────────────────────────

        [Test]
        public async Task Create_AsAdmin_Returns200()
        {
            await AuthAsAdminAsync();
            var response = await Client.PostAsJsonAsync("/Region",
                new { Name = "Asia Pacific", Currency = "JPY" });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Create_WithoutToken_Returns401()
        {
            var response = await Client.PostAsJsonAsync("/Region",
                new { Name = "Asia Pacific", Currency = "JPY" });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task Create_AsRequester_Returns403()
        {
            await AuthAsRequesterAsync();
            var response = await Client.PostAsJsonAsync("/Region",
                new { Name = "South America", Currency = "BRL" });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        // PUT /Region ─────────────────────────────────────

        [Test]
        public async Task Update_AsAdmin_ExistingRegion_Returns200()
        {
            await AuthAsAdminAsync();
            var region = await SeedRegionAsync("Old Name", "USD");

            var response = await Client.PutAsJsonAsync("/Region",
                new { Id = region.Id, Name = "New Name", Currency = "EUR" });

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Update_AsAdmin_NonExistentId_Returns400()
        {
            await AuthAsAdminAsync();
            var response = await Client.PutAsJsonAsync("/Region",
                new { Id = Guid.NewGuid(), Name = "X", Currency = "X" });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        // DELETE /Region/{id} ─────────────────────────────

        [Test]
        public async Task Delete_AsAdmin_ExistingRegion_Returns200()
        {
            await AuthAsAdminAsync();
            var region = await SeedRegionAsync("To Delete");

            var response = await Client.DeleteAsync($"/Region/{region.Id}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Delete_WithoutToken_Returns401()
        {
            var response = await Client.DeleteAsync($"/Region/{Guid.NewGuid()}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }
    }

    // ═══════════════════════════════════════════════════════
    //  ROLE
    // ═══════════════════════════════════════════════════════

    [TestFixture]
    public class RoleEndpointTests : EndpointTestBase
    {
        // GET /Role ───────────────────────────────────────

        [Test]
        public async Task GetAll_NoAuth_Returns200()
        {
            var response = await Client.GetAsync("/Role");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        // POST /Role ──────────────────────────────────────

        [Test]
        public async Task Create_AsAdmin_Returns200()
        {
            await AuthAsAdminAsync();
            var response = await Client.PostAsJsonAsync("/Role", new { Name = "Manager" });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Create_WithoutToken_Returns401()
        {
            var response = await Client.PostAsJsonAsync("/Role", new { Name = "Manager" });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task Create_AsRequester_Returns403()
        {
            await AuthAsRequesterAsync();
            var response = await Client.PostAsJsonAsync("/Role", new { Name = "Manager" });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        // PUT /Role/{id} ──────────────────────────────────

        [Test]
        public async Task Update_AsAdmin_ExistingRole_Returns200()
        {
            await AuthAsAdminAsync();
            var role = await SeedRoleAsync("OldRole");

            var response = await Client.PutAsJsonAsync($"/Role/{role.Id}", new { Name = "NewRole" });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Update_AsAdmin_NonExistentId_Returns400()
        {
            await AuthAsAdminAsync();
            var response = await Client.PutAsJsonAsync($"/Role/{Guid.NewGuid()}", new { Name = "X" });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        // DELETE /Role/{id} ───────────────────────────────

        [Test]
        public async Task Delete_AsAdmin_ExistingRole_Returns200()
        {
            await AuthAsAdminAsync();
            var role = await SeedRoleAsync("ToDelete");

            var response = await Client.DeleteAsync($"/Role/{role.Id}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Delete_WithoutToken_Returns401()
        {
            var response = await Client.DeleteAsync($"/Role/{Guid.NewGuid()}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }
    }

    // ═══════════════════════════════════════════════════════
    //  REQUEST TYPE
    // ═══════════════════════════════════════════════════════

    [TestFixture]
    public class RequestTypeEndpointTests : EndpointTestBase
    {
        // GET /RequestType ────────────────────────────────

        [Test]
        public async Task GetAll_Authenticated_Returns200()
        {
            await AuthAsRequesterAsync();
            var response = await Client.GetAsync("/RequestType");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task GetAll_WithoutToken_Returns401()
        {
            var response = await Client.GetAsync("/RequestType");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task GetAll_AfterSeeding_ReturnsSeededType()
        {
            await AuthAsRequesterAsync();
            await using var db = CreateDbContext();
            db.RequestTypes.Add(new RequestType { Id = Guid.NewGuid(), Name = "IT Equipment" });
            await db.SaveChangesAsync();

            var response = await Client.GetAsync("/RequestType");
            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            var items = json.GetProperty("data").EnumerateArray().ToList();

            Assert.That(items.Any(i => i.GetProperty("name").GetString() == "IT Equipment"), Is.True);
        }
    }
}