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
    //  PRODUCT
    // ═══════════════════════════════════════════════════════

    [TestFixture]
    public class ProductEndpointTests : EndpointTestBase
    {
        private async Task<(RequestType type, Product product)> SeedProductAsync(string name = "Laptop")
        {
            await using var db = CreateDbContext();
            var type = new RequestType { Id = Guid.NewGuid(), Name = "IT" };
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = "Desc",
                RequestType = new List<RequestType> { type }
            };
            db.RequestTypes.Add(type);
            db.Products.Add(product);
            await db.SaveChangesAsync();
            return (type, product);
        }

        // GET /Product ────────────────────────────────────

        [Test]
        public async Task GetAll_Authenticated_Returns200()
        {
            await AuthAsRequesterAsync();
            var response = await Client.GetAsync("/Product");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task GetAll_WithoutToken_Returns401()
        {
            var response = await Client.GetAsync("/Product");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        // GET /Product/{id} ───────────────────────────────

        [Test]
        public async Task GetById_ExistingProduct_Returns200()
        {
            await AuthAsRequesterAsync();
            var (_, product) = await SeedProductAsync();

            var response = await Client.GetAsync($"/Product/{product.Id}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task GetById_NonExistentId_Returns400()
        {
            await AuthAsRequesterAsync();
            var response = await Client.GetAsync($"/Product/{Guid.NewGuid()}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        // POST /Product ───────────────────────────────────

        [Test]
        public async Task Create_AsAdmin_Returns200()
        {
            await AuthAsAdminAsync();
            await using var db = CreateDbContext();
            var type = new RequestType { Id = Guid.NewGuid(), Name = "Office" };
            db.RequestTypes.Add(type);
            await db.SaveChangesAsync();

            var response = await Client.PostAsJsonAsync("/Product", new
            {
                Name = "Desk",
                Description = "Office desk",
                RequestTypeIds = new[] { type.Id }
            });

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Create_WithoutToken_Returns401()
        {
            var response = await Client.PostAsJsonAsync("/Product", new
            {
                Name = "Chair",
                Description = "Office chair",
                RequestTypeIds = Array.Empty<Guid>()
            });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task Create_AsRequester_Returns403()
        {
            await AuthAsRequesterAsync();
            var response = await Client.PostAsJsonAsync("/Product", new
            {
                Name = "Chair",
                Description = "Office chair",
                RequestTypeIds = Array.Empty<Guid>()
            });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        // PUT /Product ────────────────────────────────────

        [Test]
        public async Task Update_AsAdmin_ExistingProduct_Returns200()
        {
            await AuthAsAdminAsync();
            var (type, product) = await SeedProductAsync("OldName");

            var response = await Client.PutAsJsonAsync("/Product", new
            {
                Id = product.Id,
                Name = "NewName",
                Description = "Updated desc",
                RequestTypeIds = new[] { type.Id }
            });

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Update_AsAdmin_NonExistentId_Returns400()
        {
            await AuthAsAdminAsync();
            var response = await Client.PutAsJsonAsync("/Product", new
            {
                Id = Guid.NewGuid(),
                Name = "X",
                Description = "X",
                RequestTypeIds = Array.Empty<Guid>()
            });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        // DELETE /Product/{id} ────────────────────────────

        [Test]
        public async Task Delete_AsAdmin_ExistingProduct_Returns200()
        {
            await AuthAsAdminAsync();
            var (_, product) = await SeedProductAsync("ToDelete");

            var response = await Client.DeleteAsync($"/Product/{product.Id}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Delete_WithoutToken_Returns401()
        {
            var response = await Client.DeleteAsync($"/Product/{Guid.NewGuid()}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        // GET /Product/filtered ───────────────────────────

        [Test]
        public async Task GetFiltered_WithoutToken_Returns401()
        {
            var response = await Client.GetAsync("/Product/filtered");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }
    }

    // ═══════════════════════════════════════════════════════
    //  PRICE
    // ═══════════════════════════════════════════════════════

    [TestFixture]
    public class PriceEndpointTests : EndpointTestBase
    {
        private async Task<(Product product, Region region)> SeedProductAndRegionAsync()
        {
            await using var db = CreateDbContext();
            var type = new RequestType { Id = Guid.NewGuid(), Name = "IT" };
            var product = new Product { Id = Guid.NewGuid(), Name = "Mouse", Description = "USB mouse", RequestType = new List<RequestType> { type } };
            var region = new Region { Id = Guid.NewGuid(), Name = "Europe", Currency = "EUR" };
            db.RequestTypes.Add(type);
            db.Products.Add(product);
            db.Regions.Add(region);
            await db.SaveChangesAsync();
            return (product, region);
        }

        private async Task<Price> SeedPriceAsync(Guid productId, Guid regionId, decimal amount = 99m)
        {
            await using var db = CreateDbContext();
            var price = new Price { ProductId = productId, RegionId = regionId, Amount = amount, UnitsOfMeasure = "pcs" };
            db.Prices.Add(price);
            await db.SaveChangesAsync();
            return price;
        }

        // GET /Price ──────────────────────────────────────

        [Test]
        public async Task GetAll_Authenticated_Returns200()
        {
            await AuthAsRequesterAsync();
            var response = await Client.GetAsync("/Price");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task GetAll_WithoutToken_Returns401()
        {
            var response = await Client.GetAsync("/Price");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        // GET /Price/{productId}/{regionId} ───────────────

        [Test]
        public async Task GetById_ExistingPrice_Returns200()
        {
            await AuthAsRequesterAsync();
            var (product, region) = await SeedProductAndRegionAsync();
            await SeedPriceAsync(product.Id, region.Id);

            var response = await Client.GetAsync($"/Price/{product.Id}/{region.Id}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task GetById_NonExistentPrice_Returns404()
        {
            await AuthAsRequesterAsync();
            var response = await Client.GetAsync($"/Price/{Guid.NewGuid()}/{Guid.NewGuid()}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        // POST /Price ─────────────────────────────────────

        [Test]
        public async Task Create_AsAdmin_Returns200()
        {
            await AuthAsAdminAsync();
            var (product, region) = await SeedProductAndRegionAsync();

            var response = await Client.PostAsJsonAsync("/Price", new
            {
                ProductId = product.Id,
                RegionId = region.Id,
                Amount = 250m,
                UnitsOfMeasure = "pcs"
            });

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Create_WithoutToken_Returns401()
        {
            var response = await Client.PostAsJsonAsync("/Price", new
            {
                ProductId = Guid.NewGuid(),
                RegionId = Guid.NewGuid(),
                Amount = 10m,
                UnitsOfMeasure = "pcs"
            });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task Create_AsRequester_Returns403()
        {
            await AuthAsRequesterAsync();
            var response = await Client.PostAsJsonAsync("/Price", new
            {
                ProductId = Guid.NewGuid(),
                RegionId = Guid.NewGuid(),
                Amount = 10m,
                UnitsOfMeasure = "pcs"
            });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        // PUT /Price ──────────────────────────────────────

        [Test]
        public async Task Update_AsAdmin_ExistingPrice_Returns200()
        {
            await AuthAsAdminAsync();
            var (product, region) = await SeedProductAndRegionAsync();
            await SeedPriceAsync(product.Id, region.Id, 100m);

            var response = await Client.PutAsJsonAsync("/Price", new
            {
                ProductId = product.Id,
                RegionId = region.Id,
                Amount = 200m,
                UnitsOfMeasure = "pcs"
            });

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Update_AsAdmin_NonExistentPrice_Returns400()
        {
            await AuthAsAdminAsync();
            var response = await Client.PutAsJsonAsync("/Price", new
            {
                ProductId = Guid.NewGuid(),
                RegionId = Guid.NewGuid(),
                Amount = 1m,
                UnitsOfMeasure = "pcs"
            });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        // DELETE /Price/{productId}/{regionId} ────────────

        [Test]
        public async Task Delete_AsAdmin_ExistingPrice_Returns200()
        {
            await AuthAsAdminAsync();
            var (product, region) = await SeedProductAndRegionAsync();
            await SeedPriceAsync(product.Id, region.Id);

            var response = await Client.DeleteAsync($"/Price/{product.Id}/{region.Id}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Delete_WithoutToken_Returns401()
        {
            var response = await Client.DeleteAsync($"/Price/{Guid.NewGuid()}/{Guid.NewGuid()}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }
    }
}