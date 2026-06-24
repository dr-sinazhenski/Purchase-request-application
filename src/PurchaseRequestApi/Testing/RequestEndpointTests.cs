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
    //  REQUEST
    // ═══════════════════════════════════════════════════════

    [TestFixture]
    public class RequestEndpointTests : EndpointTestBase
    {
        private static readonly Guid NorthAmericaRegionId = Guid.Parse("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

        private async Task<RequestType> SeedRequestTypeAsync(string name = "IT Equipment")
        {
            await using var db = CreateDbContext();
            var type = new RequestType { Id = Guid.NewGuid(), Name = name };
            db.RequestTypes.Add(type);
            await db.SaveChangesAsync();
            return type;
        }

        private async Task<Product> SeedPricedProductAsync(RequestType type, string name = "Laptop", decimal price = 999m)
        {
            await using var db = CreateDbContext();

            if (!await db.Regions.AnyAsync(r => r.Id == NorthAmericaRegionId))
            {
                db.Regions.Add(new Region { Id = NorthAmericaRegionId, Name = "North America", Currency = "USD" });
            }

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = "Desc",
                RequestType = new List<RequestType> { type }
            };
            db.Products.Add(product);
            db.Prices.Add(new Price
            {
                ProductId = product.Id,
                RegionId = NorthAmericaRegionId,
                Amount = price,
                UnitsOfMeasure = "pcs"
            });

            await db.SaveChangesAsync();
            return product;
        }

        private async Task<Request> SeedRequestAsync(
            RequestType type,
            Account requester,
            string title = "Seeded Request",
            RequestStatus status = RequestStatus.Submited)
        {
            await using var db = CreateDbContext();
            var request = new Request
            {
                Id = Guid.NewGuid(),
                Title = title,
                Description = "Desc",
                Status = status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                RequestTypeId = type.Id,
                RequesterId = requester.Id,
                RequesterProducts = new List<RequesterProduct>()
            };
            db.Requests.Add(request);
            await db.SaveChangesAsync();
            return request;
        }

        // GET /Request ────────────────────────────────────

        [Test]
        public async Task GetAll_Authenticated_Returns200()
        {
            await AuthAsRequesterAsync();
            var response = await Client.GetAsync("/Request");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task GetAll_WithoutToken_Returns401()
        {
            var response = await Client.GetAsync("/Request");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task GetAll_ReturnsSeededRequest_WithExpectedFields()
        {
            var account = await AuthAsRequesterAsync();
            var type = await SeedRequestTypeAsync();
            await SeedRequestAsync(type, account, title: "Listed Request");

            var response = await Client.GetAsync("/Request");
            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            var items = json.GetProperty("data").EnumerateArray().ToList();

            var item = items.First(i => i.GetProperty("title").GetString() == "Listed Request");
            Assert.That(item.GetProperty("requestType").GetProperty("name").GetString(), Is.EqualTo(type.Name));
            Assert.That(item.GetProperty("status").GetString(), Is.EqualTo(RequestStatus.Submited.ToString()));
        }

        // GET /Request/{id} ───────────────────────────────

        [Test]
        public async Task GetById_ExistingRequest_Returns200()
        {
            var account = await AuthAsRequesterAsync();
            var type = await SeedRequestTypeAsync();
            var request = await SeedRequestAsync(type, account);

            var response = await Client.GetAsync($"/Request/{request.Id}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task GetById_ExistingRequest_ReturnsCorrectDetails()
        {
            var account = await AuthAsRequesterAsync();
            var type = await SeedRequestTypeAsync();
            var request = await SeedRequestAsync(type, account, title: "Detail Check");

            var response = await Client.GetAsync($"/Request/{request.Id}");
            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            var data = json.GetProperty("data");

            Assert.That(data.GetProperty("id").GetGuid(), Is.EqualTo(request.Id));
            Assert.That(data.GetProperty("title").GetString(), Is.EqualTo("Detail Check"));
            Assert.That(data.GetProperty("requesterId").GetGuid(), Is.EqualTo(account.Id));
        }

        [Test]
        public async Task GetById_WithoutToken_Returns401()
        {
            var response = await Client.GetAsync($"/Request/{Guid.NewGuid()}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task GetById_NonExistentId_Returns404()
        {
            // ASSUMPTION: matches Region/Price/Comment GetById-not-found (404).
            // ProductController.GetById breaks this pattern and uses 400 instead —
            // if RequestController follows Product's lead rather than the majority,
            // change this single assertion to HttpStatusCode.BadRequest.
            await AuthAsRequesterAsync();
            var response = await Client.GetAsync($"/Request/{Guid.NewGuid()}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        // GET /Request/filtered ───────────────────────────

        [Test]
        public async Task GetFiltered_WithoutToken_Returns401()
        {
            var response = await Client.GetAsync("/Request/filtered");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task GetFiltered_NoQueryParams_ReturnsAllRequests()
        {
            var account = await AuthAsRequesterAsync();
            var type = await SeedRequestTypeAsync();
            await SeedRequestAsync(type, account, title: "Unfiltered A");
            await SeedRequestAsync(type, account, title: "Unfiltered B");

            var response = await Client.GetAsync("/Request/filtered");
            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            var items = json.GetProperty("data").EnumerateArray().ToList();

            Assert.That(items.Count, Is.GreaterThanOrEqualTo(2));
        }

        [Test]
        public async Task GetFiltered_ByRequestTypeId_ReturnsOnlyMatchingType()
        {
            var account = await AuthAsRequesterAsync();
            var typeA = await SeedRequestTypeAsync("Type A");
            var typeB = await SeedRequestTypeAsync("Type B");
            await SeedRequestAsync(typeA, account, title: "From Type A");
            await SeedRequestAsync(typeB, account, title: "From Type B");

            var response = await Client.GetAsync($"/Request/filtered?requestTypeId={typeA.Id}");
            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            var items = json.GetProperty("data").EnumerateArray().ToList();

            Assert.That(items, Has.All.Matches<JsonElement>(i => i.GetProperty("requestType").GetString() == "Type A"));
        }

        [Test]
        public async Task GetFiltered_ByStatus_ReturnsOnlyMatchingStatus()
        {
            var account = await AuthAsRequesterAsync();
            var type = await SeedRequestTypeAsync();
            await SeedRequestAsync(type, account, title: "Submitted One", status: RequestStatus.Submited);
            await SeedRequestAsync(type, account, title: "Approved One", status: RequestStatus.Approved);

            var response = await Client.GetAsync("/Request/filtered?status=Approved");
            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            var items = json.GetProperty("data").EnumerateArray().ToList();

            Assert.That(items, Has.All.Matches<JsonElement>(i => i.GetProperty("status").GetString() == "Approved"));
        }

        [Test]
        public async Task GetFiltered_RegionIdParam_IsCurrentlyIgnoredByTheHandler()
        {
            // GetRequestsFilteredHandler accepts RegionId in the command but never
            // applies it as a query filter — documenting current (likely buggy)
            // behavior so this test breaks loudly the moment that gets fixed.
            var account = await AuthAsRequesterAsync();
            var type = await SeedRequestTypeAsync();
            await SeedRequestAsync(type, account, title: "Region Filter Probe");

            var response = await Client.GetAsync($"/Request/filtered?regionId={Guid.NewGuid()}");
            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            var items = json.GetProperty("data").EnumerateArray().ToList();

            Assert.That(items.Any(i => i.GetProperty("title").GetString() == "Region Filter Probe"), Is.True);
        }

        // POST /Request ───────────────────────────────────

        [Test]
        public async Task Create_Authenticated_PersistsRequest()
        {
            var account = await AuthAsRequesterAsync();
            var type = await SeedRequestTypeAsync();

            await Client.PostAsJsonAsync("/Request", new
            {
                Title = "Persisted Purchase",
                Description = "desc",
                RequestTypeId = type.Id,
                RequesterId = account.Id,
                ProductIdAmount = new Dictionary<Guid, int>()
            });

            await using var db = CreateDbContext();
            Assert.That(await db.Requests.AnyAsync(r => r.Title == "Persisted Purchase"), Is.True);
        }

        [Test]
        public async Task Create_Authenticated_DefaultsToSubmittedStatus()
        {
            var account = await AuthAsRequesterAsync();
            var type = await SeedRequestTypeAsync();

            var response = await Client.PostAsJsonAsync("/Request", new
            {
                Title = "Status Check",
                Description = "desc",
                RequestTypeId = type.Id,
                RequesterId = account.Id,
                ProductIdAmount = new Dictionary<Guid, int>()
            });

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            Assert.That(json.GetProperty("data").GetProperty("status").GetString(), Is.EqualTo(RequestStatus.Submited.ToString()));
        }

        [Test]
        public async Task Create_WithoutToken_Returns401()
        {
            var response = await Client.PostAsJsonAsync("/Request", new
            {
                Title = "X",
                Description = "X",
                RequestTypeId = Guid.NewGuid(),
                ProductIdAmount = new Dictionary<Guid, int>()
            });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task Create_NonExistentRequestTypeId_Returns400()
        {
            // ASSUMPTION: matches the mutation-failure convention used by every
            // other Create/Update/Delete action across the API (Account, ApproverProfile,
            // Comment, Region all map handler failures to 400 regardless of the
            // internal Error.ErrorCode). CreateRequestHandler's internal ErrorCode is
            // actually 404 here — if RequestController forwards that ErrorCode directly
            // instead of normalizing to BadRequest, change this to NotFound.
            var account = await AuthAsRequesterAsync();

            var response = await Client.PostAsJsonAsync("/Request", new
            {
                Title = "Bad Type",
                Description = "desc",
                RequestTypeId = Guid.NewGuid(), // non-existent
                RequesterId = account.Id,
                ProductIdAmount = new Dictionary<Guid, int>()
            });

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        // PUT /Request ────────────────────────────────────

        [Test]
        public async Task Update_Authenticated_ExistingRequest_Returns200()
        {
            var account = await AuthAsRequesterAsync();
            var type = await SeedRequestTypeAsync();
            var request = await SeedRequestAsync(type, account);

            var response = await Client.PutAsJsonAsync("/Request", new
            {
                Id = request.Id,
                Title = "Updated",
                Description = "Updated desc",
                RequestTypeId = type.Id,
                ProductIdAmount = new Dictionary<Guid, int>()
            });

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Update_WithoutToken_Returns401()
        {
            var response = await Client.PutAsJsonAsync("/Request", new
            {
                Id = Guid.NewGuid(),
                Title = "X",
                Description = "X",
                RequestTypeId = Guid.NewGuid(),
                ProductIdAmount = new Dictionary<Guid, int>()
            });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task Update_NonExistentId_Returns400()
        {
            var type = await SeedRequestTypeAsync();
            await AuthAsRequesterAsync();

            var response = await Client.PutAsJsonAsync("/Request", new
            {
                Id = Guid.NewGuid(),
                Title = "X",
                Description = "X",
                RequestTypeId = type.Id,
                ProductIdAmount = new Dictionary<Guid, int>()
            });

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task Update_ApprovedRequest_Returns400()
        {
            // UpdateRequestHandler only allows Submited/ForRevision/Resubmited.
            var account = await AuthAsRequesterAsync();
            var type = await SeedRequestTypeAsync();
            var request = await SeedRequestAsync(type, account, status: RequestStatus.Approved);

            var response = await Client.PutAsJsonAsync("/Request", new
            {
                Id = request.Id,
                Title = "Should fail",
                Description = "desc",
                RequestTypeId = type.Id,
                ProductIdAmount = new Dictionary<Guid, int>()
            });

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        // DELETE /Request/{id} ────────────────────────────

        [Test]
        public async Task Delete_Authenticated_ExistingRequest_Returns200()
        {
            var account = await AuthAsRequesterAsync();
            var type = await SeedRequestTypeAsync();
            var request = await SeedRequestAsync(type, account);

            var response = await Client.DeleteAsync($"/Request/{request.Id}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Delete_NonExistentId_Returns400()
        {
            await AuthAsRequesterAsync();
            var response = await Client.DeleteAsync($"/Request/{Guid.NewGuid()}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task Delete_WithoutToken_Returns401()
        {
            var response = await Client.DeleteAsync($"/Request/{Guid.NewGuid()}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task Approve_SubmittedRequest_Returns200()
        {
            var requester = await AuthAsRequesterAsync();
            var type = await SeedRequestTypeAsync();
            var request = await SeedRequestAsync(type, requester, status: RequestStatus.Submited);

            await AuthAsApproverAsync(); // switch the bearer token to a role allowed to approve

            var response = await Client.PutAsync($"/Approve/{request.Id}", null);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Approve_SubmittedRequest_ChangesStatusToApproved()
        {
            var requester = await AuthAsRequesterAsync();
            var type = await SeedRequestTypeAsync();
            var request = await SeedRequestAsync(type, requester, status: RequestStatus.Submited);

            await AuthAsApproverAsync();
            await Client.PutAsync($"/Approve/{request.Id}", null);

            await using var db = CreateDbContext();
            var updated = await db.Requests.FindAsync(request.Id);
            Assert.That(updated!.Status, Is.EqualTo(RequestStatus.Approved));
        }

        [Test]
        public async Task Approve_AlreadyApprovedRequest_Returns400()
        {
            var requester = await AuthAsRequesterAsync();
            var type = await SeedRequestTypeAsync();
            var request = await SeedRequestAsync(type, requester, status: RequestStatus.Approved);

            await AuthAsApproverAsync();
            var response = await Client.PutAsync($"/Approve/{request.Id}", null);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task Approve_NonExistentId_Returns400()
        {
            await AuthAsApproverAsync();
            var response = await Client.PutAsync($"/Approve/{Guid.NewGuid()}", null);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task Reject_SubmittedRequest_NotFinal_Returns200()
        {
            var requester = await AuthAsRequesterAsync();
            var type = await SeedRequestTypeAsync();
            var request = await SeedRequestAsync(type, requester, status: RequestStatus.Submited);

            await AuthAsApproverAsync();
            var response = await Client.PutAsJsonAsync("/Reject", new { Id = request.Id, Reason = "Missing detail", IsFinal = false });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Reject_NotFinal_SetsStatusToForRevision()
        {
            var requester = await AuthAsRequesterAsync();
            var type = await SeedRequestTypeAsync();
            var request = await SeedRequestAsync(type, requester, status: RequestStatus.Submited);

            await AuthAsApproverAsync();
            await Client.PutAsJsonAsync("/Reject", new { Id = request.Id, Reason = "Fix this", IsFinal = false });

            await using var db = CreateDbContext();
            var updated = await db.Requests.FindAsync(request.Id);
            Assert.That(updated!.Status, Is.EqualTo(RequestStatus.ForRevision));
        }

        [Test]
        public async Task Reject_IsFinal_SetsStatusToRejected()
        {
            var requester = await AuthAsRequesterAsync();
            var type = await SeedRequestTypeAsync();
            var request = await SeedRequestAsync(type, requester, status: RequestStatus.Submited);

            await AuthAsApproverAsync();
            await Client.PutAsJsonAsync("/Reject", new { Id = request.Id, Reason = "Final no", IsFinal = true });

            await using var db = CreateDbContext();
            var updated = await db.Requests.FindAsync(request.Id);
            Assert.That(updated!.Status, Is.EqualTo(RequestStatus.Rejected));
        }

        [Test]
        public async Task Reject_NonExistentId_Returns400()
        {
            await AuthAsApproverAsync();
            var response = await Client.PutAsJsonAsync("/Reject", new { Id = Guid.NewGuid(), Reason = "Nope", IsFinal = false });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task Reject_ApprovedRequest_Returns400()
        {
            var requester = await AuthAsRequesterAsync();
            var type = await SeedRequestTypeAsync();
            var request = await SeedRequestAsync(type, requester, status: RequestStatus.Approved);

            await AuthAsApproverAsync();
            var response = await Client.PutAsJsonAsync("/Reject", new { Id = request.Id, Reason = "Too late", IsFinal = false });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task Approve_WithoutToken_Returns401()
        {
            var response = await Client.PutAsync($"/Approve/{Guid.NewGuid()}", null);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task Reject_WithoutToken_Returns401()
        {
            var response = await Client.PutAsJsonAsync("/Reject", new { Id = Guid.NewGuid(), Reason = "X", IsFinal = false });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }
    }
}