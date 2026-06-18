using Application.BusinessLogic.CommentLogic.CreateComment;
using Application.BusinessLogic.CommentLogic.DeleteComment;
using Application.BusinessLogic.CommentLogic.Dto;
using Application.BusinessLogic.CommentLogic.GetAllComments;
using Application.BusinessLogic.CommentLogic.GetAllRequestComments;
using Application.BusinessLogic.CommentLogic.GetComment;
using Application.BusinessLogic.CommentLogic.UpdateComment;
using Infrastructure.Database;
using Infrastructure.Database.Entities;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Testing;

namespace Testing.CommentTests
{
    [TestFixture]
    public class CreateCommentTests : HandlerTestBase
    {
        private Request _request = default!;
        private Account _account = default!;

        protected override async Task SeedAsync(AppDbContext db)
        {
            var region = DBSeeder.SeedRegion(db);
            _account = DBSeeder.SeedAccount(db, region);
            var requestType = DBSeeder.SeedRequestType(db);
            _request = DBSeeder.SeedRequest(db, requestType, _account);
        }

        [Test]
        public async Task Handle_ValidDtoWithAccount_ReturnsSuccess()
        {
            var dto = new CrudCommentDto
            {
                RequestId = _request.Id,
                AccountId = _account.Id,
                Text = "Hello"
            };

            var result = await Mediator.Send(new CreateCommentCommand(dto));

            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public async Task Handle_ValidDtoWithoutAccount_ReturnsSuccess()
        {
            var dto = new CrudCommentDto
            {
                RequestId = _request.Id,
                AccountId = null,
                Text = "Anonymous comment"
            };

            var result = await Mediator.Send(new CreateCommentCommand(dto));

            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public async Task Handle_ValidDto_PersistsCommentToDatabase()
        {
            var dto = new CrudCommentDto
            {
                RequestId = _request.Id,
                AccountId = _account.Id,
                Text = "Persisted"
            };

            await Mediator.Send(new CreateCommentCommand(dto));

            var count = await Database.Comments.CountAsync(c => c.Text == "Persisted");
            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public async Task Handle_ValidDto_ReturnsDtoWithCorrectFields()
        {
            var dto = new CrudCommentDto
            {
                RequestId = _request.Id,
                AccountId = _account.Id,
                Text = "Check fields"
            };

            var result = await Mediator.Send(new CreateCommentCommand(dto));

            Assert.That(result.Data!.RequestId, Is.EqualTo(_request.Id));
            Assert.That(result.Data.AccountId, Is.EqualTo(_account.Id));
            Assert.That(result.Data.Text, Is.EqualTo("Check fields"));
            Assert.That(result.Data.Id, Is.Not.EqualTo(Guid.Empty));
        }

        [Test]
        public async Task Handle_NonExistentRequestId_ReturnsFailure()
        {
            var dto = new CrudCommentDto
            {
                RequestId = Guid.NewGuid(),
                AccountId = _account.Id,
                Text = "Bad request id"
            };

            var result = await Mediator.Send(new CreateCommentCommand(dto));

            Assert.That(result.IsSuccess, Is.False);
        }

        [Test]
        public async Task Handle_NonExistentRequestId_Returns404Error()
        {
            var dto = new CrudCommentDto
            {
                RequestId = Guid.NewGuid(),
                Text = "Bad"
            };

            var result = await Mediator.Send(new CreateCommentCommand(dto));

            Assert.That(result.Error!.ErrorCode, Is.EqualTo(404));
        }

        [Test]
        public async Task Handle_NonExistentAccountId_ReturnsFailure()
        {
            var dto = new CrudCommentDto
            {
                RequestId = _request.Id,
                AccountId = Guid.NewGuid(), // non-existent account
                Text = "Bad account"
            };

            var result = await Mediator.Send(new CreateCommentCommand(dto));

            Assert.That(result.IsSuccess, Is.False);
        }

        [Test]
        public async Task Handle_NonExistentAccountId_Returns404Error()
        {
            var dto = new CrudCommentDto
            {
                RequestId = _request.Id,
                AccountId = Guid.NewGuid(),
                Text = "Bad account"
            };

            var result = await Mediator.Send(new CreateCommentCommand(dto));

            Assert.That(result.Error!.ErrorCode, Is.EqualTo(404));
        }
    }

    [TestFixture]
    public class DeleteCommentTests : HandlerTestBase
    {
        private Comment _comment = default!;

        protected override async Task SeedAsync(AppDbContext db)
        {
            var region = DBSeeder.SeedRegion(db);
            var account = DBSeeder.SeedAccount(db, region);
            var requestType = DBSeeder.SeedRequestType(db);
            var request = DBSeeder.SeedRequest(db, requestType, account);
            _comment = DBSeeder.SeedComment(db, request, account, text: "To be deleted");
        }

        [Test]
        public async Task Handle_ExistingComment_ReturnsSuccess()
        {
            var result = await Mediator.Send(new DeleteCommentCommand(_comment.Id));

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.True);
        }

        [Test]
        public async Task Handle_ExistingComment_RemovesFromDatabase()
        {
            await Mediator.Send(new DeleteCommentCommand(_comment.Id));

            var exists = await Database.Comments.AnyAsync(c => c.Id == _comment.Id);
            Assert.That(exists, Is.False);
        }

        [Test]
        public async Task Handle_NonExistentId_ReturnsFailure()
        {
            var result = await Mediator.Send(new DeleteCommentCommand(Guid.NewGuid()));

            Assert.That(result.IsSuccess, Is.False);
        }

        [Test]
        public async Task Handle_NonExistentId_Returns404Error()
        {
            var result = await Mediator.Send(new DeleteCommentCommand(Guid.NewGuid()));

            Assert.That(result.Error!.ErrorCode, Is.EqualTo(404));
        }
    }

    [TestFixture]
    public class UpdateCommentTests : HandlerTestBase
    {
        private Comment _comment = default!;

        protected override async Task SeedAsync(AppDbContext db)
        {
            var region = DBSeeder.SeedRegion(db);
            var account = DBSeeder.SeedAccount(db, region);
            var requestType = DBSeeder.SeedRequestType(db);
            var request = DBSeeder.SeedRequest(db, requestType, account);
            _comment = DBSeeder.SeedComment(db, request, account, text: "Original text");
        }

        [Test]
        public async Task Handle_ExistingComment_ReturnsSuccess()
        {
            var dto = new CrudCommentDto { Id = _comment.Id, Text = "Updated text" };

            var result = await Mediator.Send(new UpdateCommentCommand(dto));

            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public async Task Handle_ExistingComment_UpdatesTextInDatabase()
        {
            var dto = new CrudCommentDto { Id = _comment.Id, Text = "New text" };

            await Mediator.Send(new UpdateCommentCommand(dto));

            var updated = await Database.Comments.FindAsync(_comment.Id);
            Assert.That(updated!.Text, Is.EqualTo("New text"));
        }

        [Test]
        public async Task Handle_ExistingComment_ReturnsDtoWithUpdatedText()
        {
            var dto = new CrudCommentDto { Id = _comment.Id, Text = "Returned text" };

            var result = await Mediator.Send(new UpdateCommentCommand(dto));

            Assert.That(result.Data!.Text, Is.EqualTo("Returned text"));
            Assert.That(result.Data.Id, Is.EqualTo(_comment.Id));
        }

        [Test]
        public async Task Handle_NonExistentId_ReturnsFailure()
        {
            var dto = new CrudCommentDto { Id = Guid.NewGuid(), Text = "Nope" };

            var result = await Mediator.Send(new UpdateCommentCommand(dto));

            Assert.That(result.IsSuccess, Is.False);
        }

        [Test]
        public async Task Handle_NonExistentId_Returns404Error()
        {
            var dto = new CrudCommentDto { Id = Guid.NewGuid(), Text = "Nope" };

            var result = await Mediator.Send(new UpdateCommentCommand(dto));

            Assert.That(result.Error!.ErrorCode, Is.EqualTo(404));
        }
    }

    [TestFixture]
    public class GetCommentTests : HandlerTestBase
    {
        private Comment _comment = default!;

        protected override async Task SeedAsync(AppDbContext db)
        {
            var region = DBSeeder.SeedRegion(db);
            var account = DBSeeder.SeedAccount(db, region);
            var requestType = DBSeeder.SeedRequestType(db);
            var request = DBSeeder.SeedRequest(db, requestType, account);
            _comment = DBSeeder.SeedComment(db, request, account, text: "Findable comment");
        }

        [Test]
        public async Task Handle_ExistingComment_ReturnsSuccess()
        {
            var result = await Mediator.Send(new GetCommentCommand(_comment.Id));

            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public async Task Handle_ExistingComment_ReturnsCorrectDto()
        {
            var result = await Mediator.Send(new GetCommentCommand(_comment.Id));

            Assert.That(result.Data!.Id, Is.EqualTo(_comment.Id));
            Assert.That(result.Data.Text, Is.EqualTo("Findable comment"));
            Assert.That(result.Data.RequestId, Is.EqualTo(_comment.RequestId));
        }

        [Test]
        public async Task Handle_NonExistentId_ReturnsFailure()
        {
            var result = await Mediator.Send(new GetCommentCommand(Guid.NewGuid()));

            Assert.That(result.IsSuccess, Is.False);
        }

        [Test]
        public async Task Handle_NonExistentId_Returns404Error()
        {
            var result = await Mediator.Send(new GetCommentCommand(Guid.NewGuid()));

            Assert.That(result.Error!.ErrorCode, Is.EqualTo(404));
        }
    }

    [TestFixture]
    public class GetAllCommentsTests : HandlerTestBase
    {
        protected override async Task SeedAsync(AppDbContext db)
        {
            var region = DBSeeder.SeedRegion(db);
            var account = DBSeeder.SeedAccount(db, region);
            var requestType = DBSeeder.SeedRequestType(db);
            var request = DBSeeder.SeedRequest(db, requestType, account);
            DBSeeder.SeedComment(db, request, account, text: "Comment A");
            DBSeeder.SeedComment(db, request, account, text: "Comment B");
        }

        [Test]
        public async Task Handle_ReturnsSuccess()
        {
            var result = await Mediator.Send(new GetAllCommentsCommand());

            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public async Task Handle_ReturnsAllSeededComments()
        {
            var result = await Mediator.Send(new GetAllCommentsCommand());

            Assert.That(result.Data, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task Handle_WhenNoComments_ReturnsEmptyList()
        {
            // TearDown resets between tests, so this is a fresh DB with no seed
            // We explicitly verify empty state here by querying with a fresh fixture
            await Database.Comments.ExecuteDeleteAsync();

            var result = await Mediator.Send(new GetAllCommentsCommand());

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Empty);
        }
    }

    [TestFixture]
    public class GetAllRequestCommentsTests : HandlerTestBase
    {
        private Request _request1 = default!;
        private Request _request2 = default!;

        protected override async Task SeedAsync(AppDbContext db)
        {
            var region = DBSeeder.SeedRegion(db);
            var account = DBSeeder.SeedAccount(db, region);
            var requestType = DBSeeder.SeedRequestType(db);

            _request1 = DBSeeder.SeedRequest(db, requestType, account, title: "Request 1");
            _request2 = DBSeeder.SeedRequest(db, requestType, account, title: "Request 2");

            DBSeeder.SeedComment(db, _request1, account, text: "R1 comment 1");
            DBSeeder.SeedComment(db, _request1, account, text: "R1 comment 2");
            DBSeeder.SeedComment(db, _request2, account, text: "R2 comment 1");
        }

        [Test]
        public async Task Handle_ExistingRequest_ReturnsSuccess()
        {
            var result = await Mediator.Send(new GetAllRequestCommentsCommand(_request1.Id));

            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public async Task Handle_OnlyReturnsCommentsForSpecifiedRequest()
        {
            var result = await Mediator.Send(new GetAllRequestCommentsCommand(_request1.Id));

            Assert.That(result.Data, Has.Count.EqualTo(2));
            Assert.That(result.Data!.All(c => c.RequestId == _request1.Id), Is.True);
        }

        [Test]
        public async Task Handle_DoesNotReturnCommentsFromOtherRequests()
        {
            var result = await Mediator.Send(new GetAllRequestCommentsCommand(_request2.Id));

            Assert.That(result.Data, Has.Count.EqualTo(1));
            Assert.That(result.Data![0].Text, Is.EqualTo("R2 comment 1"));
        }

        [Test]
        public async Task Handle_RequestWithNoComments_ReturnsEmptyList()
        {
            var region = DBSeeder.SeedRegion(Database, name: "Empty Region", currency: "EUR");
            var account = DBSeeder.SeedAccount(Database, region, login: "empty.user");
            var requestType = DBSeeder.SeedRequestType(Database, name: "Empty Type");
            var emptyRequest = DBSeeder.SeedRequest(Database, requestType, account, title: "Empty");

            var result = await Mediator.Send(new GetAllRequestCommentsCommand(emptyRequest.Id));

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Empty);
        }

        [Test]
        public async Task Handle_NonExistentRequestId_ReturnsEmptyList()
        {
            // Handler doesn't validate request existence — returns empty list for unknown IDs
            var result = await Mediator.Send(new GetAllRequestCommentsCommand(Guid.NewGuid()));

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Empty);
        }
    }
}