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
    [TestFixture]
    public class CommentEndpointTests : EndpointTestBase
    {
        private async Task<(RequestType type, Request request)> SeedTypeAndRequestAsync(Account requester)
        {
            await using var db = CreateDbContext();
            var type = new RequestType { Id = Guid.NewGuid(), Name = "IT" };
            var request = new Request
            {
                Id = Guid.NewGuid(),
                Title = "Comment target",
                Description = "Desc",
                Status = RequestStatus.Submited,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                RequestTypeId = type.Id,
                RequesterId = requester.Id
            };
            db.RequestTypes.Add(type);
            db.Requests.Add(request);
            await db.SaveChangesAsync();
            return (type, request);
        }

        private async Task<Comment> SeedCommentAsync(Request request, Account? account, string text = "Existing comment")
        {
            await using var db = CreateDbContext();
            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                RequestId = request.Id,
                AccountId = account?.Id,
                Text = text,
                CreationTime = DateTime.UtcNow
            };
            db.Comments.Add(comment);
            await db.SaveChangesAsync();
            return comment;
        }

        // POST /Comment ───────────────────────────────────

        [Test]
        public async Task Create_Authenticated_ValidDto_Returns200()
        {
            var account = await AuthAsRequesterAsync();
            var (_, request) = await SeedTypeAndRequestAsync(account);

            var response = await Client.PostAsJsonAsync("/Comment", new
            {
                RequestId = request.Id,
                AccountId = account.Id,
                Text = "Hello there"
            });

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Create_Authenticated_PersistsComment()
        {
            var account = await AuthAsRequesterAsync();
            var (_, request) = await SeedTypeAndRequestAsync(account);

            await Client.PostAsJsonAsync("/Comment", new
            {
                RequestId = request.Id,
                AccountId = account.Id,
                Text = "Persisted comment"
            });

            await using var db = CreateDbContext();
            Assert.That(await db.Comments.AnyAsync(c => c.Text == "Persisted comment"), Is.True);
        }

        [Test]
        public async Task Create_WithoutToken_Returns401()
        {
            var response = await Client.PostAsJsonAsync("/Comment", new
            {
                RequestId = Guid.NewGuid(),
                Text = "X"
            });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task Create_NonExistentRequestId_Returns400()
        {
            await AuthAsRequesterAsync();
            var response = await Client.PostAsJsonAsync("/Comment", new
            {
                RequestId = Guid.NewGuid(),
                Text = "Orphan comment"
            });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        // GET /Comment ────────────────────────────────────

        [Test]
        public async Task GetAll_Authenticated_Returns200()
        {
            await AuthAsRequesterAsync();
            var response = await Client.GetAsync("/Comment");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task GetAll_WithoutToken_Returns401()
        {
            var response = await Client.GetAsync("/Comment");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        // GET /Comment/{id} ───────────────────────────────

        [Test]
        public async Task GetById_ExistingComment_Returns200()
        {
            var account = await AuthAsRequesterAsync();
            var (_, request) = await SeedTypeAndRequestAsync(account);
            var comment = await SeedCommentAsync(request, account);

            var response = await Client.GetAsync($"/Comment/{comment.Id}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task GetById_NonExistentId_Returns404()
        {
            await AuthAsRequesterAsync();
            var response = await Client.GetAsync($"/Comment/{Guid.NewGuid()}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task GetById_WithoutToken_Returns401()
        {
            var response = await Client.GetAsync($"/Comment/{Guid.NewGuid()}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        // GET /Comment/request/{id} ───────────────────────

        [Test]
        public async Task GetByRequestId_ReturnsOnlyMatchingComments()
        {
            var account = await AuthAsRequesterAsync();
            var (_, request) = await SeedTypeAndRequestAsync(account);
            await SeedCommentAsync(request, account, "First");
            await SeedCommentAsync(request, account, "Second");

            var response = await Client.GetAsync($"/Comment/request/{request.Id}");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            var items = json.GetProperty("data").EnumerateArray().ToList();
            Assert.That(items, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task GetByRequestId_WithoutToken_Returns401()
        {
            var response = await Client.GetAsync($"/Comment/request/{Guid.NewGuid()}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        // PUT /Comment ────────────────────────────────────

        [Test]
        public async Task Update_Authenticated_ExistingComment_Returns200()
        {
            var account = await AuthAsRequesterAsync();
            var (_, request) = await SeedTypeAndRequestAsync(account);
            var comment = await SeedCommentAsync(request, account, "Original");

            var response = await Client.PutAsJsonAsync("/Comment", new
            {
                Id = comment.Id,
                RequestId = request.Id,
                Text = "Updated text"
            });

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Update_NonExistentId_Returns400()
        {
            await AuthAsRequesterAsync();
            var response = await Client.PutAsJsonAsync("/Comment", new
            {
                Id = Guid.NewGuid(),
                Text = "Nope"
            });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task Update_WithoutToken_Returns401()
        {
            var response = await Client.PutAsJsonAsync("/Comment", new
            {
                Id = Guid.NewGuid(),
                Text = "Nope"
            });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        // DELETE /Comment/{id} ────────────────────────────

        [Test]
        public async Task Delete_Authenticated_ExistingComment_Returns200()
        {
            var account = await AuthAsRequesterAsync();
            var (_, request) = await SeedTypeAndRequestAsync(account);
            var comment = await SeedCommentAsync(request, account, "To delete");

            var response = await Client.DeleteAsync($"/Comment/{comment.Id}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Delete_NonExistentId_Returns400()
        {
            await AuthAsRequesterAsync();
            var response = await Client.DeleteAsync($"/Comment/{Guid.NewGuid()}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task Delete_WithoutToken_Returns401()
        {
            var response = await Client.DeleteAsync($"/Comment/{Guid.NewGuid()}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }
    }
}