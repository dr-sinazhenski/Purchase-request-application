using Application.BusinessLogic.RequestLogic.GetAllRequests;
using Application.BusinessLogic.RequestLogic.RejectRequest;
using Application.BusinessLogic.RequestLogic.UpdateRequest;
using Application.BusinessLogic.RequestLogic.Dto;
using Infrastructure.Database;
using Infrastructure.Database.Entities;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Testing;

namespace Testing.RequestTests
{
    // ═══════════════════════════════════════════════════════
    //  UPDATE REQUEST
    // ═══════════════════════════════════════════════════════

    [TestFixture]
    public class UpdateRequestTests : HandlerTestBase
    {
        private Region _region = default!;
        private Account _account = default!;
        private RequestType _requestType = default!;
        private Product _product = default!;

        protected override async Task SeedAsync(AppDbContext db)
        {
            _region = DBSeeder.SeedRegion(db);
            _account = DBSeeder.SeedAccount(db, _region);
            _requestType = DBSeeder.SeedRequestType(db, name: "IT Products");
            _product = DBSeeder.SeedProduct(db, _requestType, name: "Laptop");
            DBSeeder.SeedNorthAmericaPrice(db, _product.Id, amount: 999m);
        }

        private Request SeedSubmittedRequest(RequestStatus status = RequestStatus.Submited)
        {
            var request = DBSeeder.SeedRequest(Database, _requestType, _account, status: status);

            var rp = new RequesterProduct
            {
                RequestId = request.Id,
                Request = request,
                ProductId = _product.Id,
                Product = _product,
                Quantity = 1
            };
            Database.RequesterProducts.Add(rp);
            Database.SaveChanges();

            return request;
        }

        [Test]
        public async Task Handle_ExistingSubmittedRequest_ReturnsSuccess()
        {
            var request = SeedSubmittedRequest();

            var dto = new CreateRequestDto
            {
                Id = request.Id,
                Title = "Updated title",
                Description = "Updated description",
                RequestTypeId = _requestType.Id,
                ProductIdAmount = new Dictionary<Guid, int> { { _product.Id, 1 } }
            };

            var result = await Mediator.Send(new UpdateRequestCommand(dto));

            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public async Task Handle_ValidUpdate_PersistsTitleAndDescription()
        {
            var request = SeedSubmittedRequest();

            var dto = new CreateRequestDto
            {
                Id = request.Id,
                Title = "New title",
                Description = "New description",
                RequestTypeId = _requestType.Id,
                ProductIdAmount = new Dictionary<Guid, int> { { _product.Id, 1 } }
            };

            await Mediator.Send(new UpdateRequestCommand(dto));

            var updated = await Database.Requests.FindAsync(request.Id);
            Assert.That(updated!.Title, Is.EqualTo("New title"));
            Assert.That(updated.Description, Is.EqualTo("New description"));
        }

        [Test]
        public async Task Handle_NonExistentRequestId_ReturnsFailure()
        {
            var dto = new CreateRequestDto
            {
                Id = Guid.NewGuid(),
                Title = "X",
                Description = "X",
                RequestTypeId = _requestType.Id,
                ProductIdAmount = new Dictionary<Guid, int>()
            };

            var result = await Mediator.Send(new UpdateRequestCommand(dto));

            Assert.That(result.IsSuccess, Is.False);
        }

        [Test]
        public async Task Handle_NonExistentRequestTypeId_ReturnsFailure()
        {
            var request = SeedSubmittedRequest();

            var dto = new CreateRequestDto
            {
                Id = request.Id,
                Title = "X",
                Description = "X",
                RequestTypeId = Guid.NewGuid(), // non-existent
                ProductIdAmount = new Dictionary<Guid, int>()
            };

            var result = await Mediator.Send(new UpdateRequestCommand(dto));

            Assert.That(result.IsSuccess, Is.False);
        }

        [Test]
        public async Task Handle_ApprovedRequest_ReturnsFailure()
        {
            var request = SeedSubmittedRequest(RequestStatus.Approved);

            var dto = new CreateRequestDto
            {
                Id = request.Id,
                Title = "X",
                Description = "X",
                RequestTypeId = _requestType.Id,
                ProductIdAmount = new Dictionary<Guid, int>()
            };

            var result = await Mediator.Send(new UpdateRequestCommand(dto));

            Assert.That(result.IsSuccess, Is.False);
        }

        [Test]
        public async Task Handle_RejectedRequest_ReturnsFailure()
        {
            var request = SeedSubmittedRequest(RequestStatus.Rejected);

            var dto = new CreateRequestDto
            {
                Id = request.Id,
                Title = "X",
                Description = "X",
                RequestTypeId = _requestType.Id,
                ProductIdAmount = new Dictionary<Guid, int>()
            };

            var result = await Mediator.Send(new UpdateRequestCommand(dto));

            Assert.That(result.IsSuccess, Is.False);
        }

        [Test]
        public async Task Handle_ForRevisionRequest_TransitionsToResubmited()
        {
            var request = SeedSubmittedRequest(RequestStatus.ForRevision);

            var dto = new CreateRequestDto
            {
                Id = request.Id,
                Title = "Resubmit me",
                Description = "desc",
                RequestTypeId = _requestType.Id,
                ProductIdAmount = new Dictionary<Guid, int> { { _product.Id, 1 } }
            };

            await Mediator.Send(new UpdateRequestCommand(dto));

            var updated = await Database.Requests.FindAsync(request.Id);
            Assert.That(updated!.Status, Is.EqualTo(RequestStatus.Resubmited));
        }

        [Test]
        public async Task Handle_SubmittedRequest_StatusUnchanged()
        {
            var request = SeedSubmittedRequest(RequestStatus.Submited);

            var dto = new CreateRequestDto
            {
                Id = request.Id,
                Title = "Still submitted",
                Description = "desc",
                RequestTypeId = _requestType.Id,
                ProductIdAmount = new Dictionary<Guid, int> { { _product.Id, 1 } }
            };

            await Mediator.Send(new UpdateRequestCommand(dto));

            var updated = await Database.Requests.FindAsync(request.Id);
            Assert.That(updated!.Status, Is.EqualTo(RequestStatus.Submited));
        }

        [Test]
        public async Task Handle_IncreasesQuantityOfExistingProductLine()
        {
            var request = SeedSubmittedRequest();

            var dto = new CreateRequestDto
            {
                Id = request.Id,
                Title = "Qty bump",
                Description = "desc",
                RequestTypeId = _requestType.Id,
                ProductIdAmount = new Dictionary<Guid, int> { { _product.Id, 5 } }
            };

            await Mediator.Send(new UpdateRequestCommand(dto));

            var rp = await Database.RequesterProducts
                .FirstAsync(x => x.RequestId == request.Id && x.ProductId == _product.Id);
            Assert.That(rp.Quantity, Is.EqualTo(5));
        }

        [Test]
        public async Task Handle_ProductRemovedFromDto_IsDeletedFromRequesterProducts()
        {
            var request = SeedSubmittedRequest();

            var dto = new CreateRequestDto
            {
                Id = request.Id,
                Title = "Drop product",
                Description = "desc",
                RequestTypeId = _requestType.Id,
                ProductIdAmount = new Dictionary<Guid, int>() // empty: removes existing line
            };

            await Mediator.Send(new UpdateRequestCommand(dto));

            var stillThere = await Database.RequesterProducts
                .AnyAsync(x => x.RequestId == request.Id && x.ProductId == _product.Id);
            Assert.That(stillThere, Is.False);
        }

        [Test]
        public async Task Handle_AddsNewProductLine()
        {
            var request = SeedSubmittedRequest();
            var newProduct = DBSeeder.SeedProduct(Database, _requestType, name: "Monitor");
            DBSeeder.SeedNorthAmericaPrice(Database, newProduct.Id, amount: 250m);

            var dto = new CreateRequestDto
            {
                Id = request.Id,
                Title = "Add product",
                Description = "desc",
                RequestTypeId = _requestType.Id,
                ProductIdAmount = new Dictionary<Guid, int>
                {
                    { _product.Id, 1 },
                    { newProduct.Id, 2 }
                }
            };

            var result = await Mediator.Send(new UpdateRequestCommand(dto));

            Assert.That(result.IsSuccess, Is.True);
            var rp = await Database.RequesterProducts
                .FirstOrDefaultAsync(x => x.RequestId == request.Id && x.ProductId == newProduct.Id);
            Assert.That(rp, Is.Not.Null);
            Assert.That(rp!.Quantity, Is.EqualTo(2));
        }

        [Test]
        public async Task Handle_ReturnsDtoWithUpdatedFields()
        {
            var request = SeedSubmittedRequest();

            var dto = new CreateRequestDto
            {
                Id = request.Id,
                Title = "Returned title",
                Description = "Returned description",
                RequestTypeId = _requestType.Id,
                ProductIdAmount = new Dictionary<Guid, int> { { _product.Id, 3 } }
            };

            var result = await Mediator.Send(new UpdateRequestCommand(dto));

            Assert.That(result.Data!.Title, Is.EqualTo("Returned title"));
            Assert.That(result.Data.Description, Is.EqualTo("Returned description"));
            Assert.That(result.Data.Products, Has.Count.EqualTo(1));
            Assert.That(result.Data.Products[0].Amount, Is.EqualTo(3));
            Assert.That(result.Data.Products[0].Price, Is.EqualTo(999m));
        }
    }

    // ═══════════════════════════════════════════════════════
    //  REJECT REQUEST
    // ═══════════════════════════════════════════════════════

    [TestFixture]
    public class RejectRequestTests : HandlerTestBase
    {
        private RequestType _requestType = default!;
        private Account _account = default!;

        protected override async Task SeedAsync(AppDbContext db)
        {
            var region = DBSeeder.SeedRegion(db);
            _account = DBSeeder.SeedAccount(db, region);
            _requestType = DBSeeder.SeedRequestType(db);
        }

        [Test]
        public async Task Handle_SubmittedRequest_ReturnsSuccess()
        {
            var request = DBSeeder.SeedRequest(Database, _requestType, _account, status: RequestStatus.Submited);
            var dto = new RejectRequestDto { Id = request.Id, Reason = "Missing budget approval", IsFinal = false };

            var result = await Mediator.Send(new RejectRequestCommand(dto));

            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public async Task Handle_ResubmittedRequest_ReturnsSuccess()
        {
            var request = DBSeeder.SeedRequest(Database, _requestType, _account, status: RequestStatus.Resubmited);
            var dto = new RejectRequestDto { Id = request.Id, Reason = "Still wrong", IsFinal = false };

            var result = await Mediator.Send(new RejectRequestCommand(dto));

            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public async Task Handle_NotFinal_SetsStatusToForRevision()
        {
            var request = DBSeeder.SeedRequest(Database, _requestType, _account, status: RequestStatus.Submited);
            var dto = new RejectRequestDto { Id = request.Id, Reason = "Needs more detail", IsFinal = false };

            await Mediator.Send(new RejectRequestCommand(dto));

            var updated = await Database.Requests.FindAsync(request.Id);
            Assert.That(updated!.Status, Is.EqualTo(RequestStatus.ForRevision));
        }

        [Test]
        public async Task Handle_IsFinal_SetsStatusToRejected()
        {
            var request = DBSeeder.SeedRequest(Database, _requestType, _account, status: RequestStatus.Submited);
            var dto = new RejectRequestDto { Id = request.Id, Reason = "Budget cut", IsFinal = true };

            await Mediator.Send(new RejectRequestCommand(dto));

            var updated = await Database.Requests.FindAsync(request.Id);
            Assert.That(updated!.Status, Is.EqualTo(RequestStatus.Rejected));
        }

        [Test]
        public async Task Handle_CreatesCommentWithReasonText()
        {
            var request = DBSeeder.SeedRequest(Database, _requestType, _account, status: RequestStatus.Submited);
            var dto = new RejectRequestDto { Id = request.Id, Reason = "Wrong product line", IsFinal = false };

            await Mediator.Send(new RejectRequestCommand(dto));

            var comment = await Database.Comments.FirstOrDefaultAsync(c => c.RequestId == request.Id);
            Assert.That(comment, Is.Not.Null);
            Assert.That(comment!.Text, Is.EqualTo("Wrong product line"));
        }

        [Test]
        public async Task Handle_SetsRejectionCommentIdOnRequest()
        {
            var request = DBSeeder.SeedRequest(Database, _requestType, _account, status: RequestStatus.Submited);
            var dto = new RejectRequestDto { Id = request.Id, Reason = "Linked comment check", IsFinal = false };

            await Mediator.Send(new RejectRequestCommand(dto));

            var updated = await Database.Requests.FindAsync(request.Id);
            var comment = await Database.Comments.FirstAsync(c => c.RequestId == request.Id);
            Assert.That(updated!.RejectionCommentId, Is.EqualTo(comment.Id));
        }

        [Test]
        public async Task Handle_NonExistentId_ReturnsFailure()
        {
            var dto = new RejectRequestDto { Id = Guid.NewGuid(), Reason = "Nope", IsFinal = false };

            var result = await Mediator.Send(new RejectRequestCommand(dto));

            Assert.That(result.IsSuccess, Is.False);
        }

        [Test]
        public async Task Handle_NonExistentId_Returns404Error()
        {
            var dto = new RejectRequestDto { Id = Guid.NewGuid(), Reason = "Nope", IsFinal = false };

            var result = await Mediator.Send(new RejectRequestCommand(dto));

            Assert.That(result.Error!.ErrorCode, Is.EqualTo(404));
        }

        [Test]
        public async Task Handle_ApprovedRequest_ReturnsFailure()
        {
            var request = DBSeeder.SeedRequest(Database, _requestType, _account, status: RequestStatus.Approved);
            var dto = new RejectRequestDto { Id = request.Id, Reason = "Too late", IsFinal = false };

            var result = await Mediator.Send(new RejectRequestCommand(dto));

            Assert.That(result.IsSuccess, Is.False);
        }

        [Test]
        public async Task Handle_ApprovedRequest_Returns400Error()
        {
            var request = DBSeeder.SeedRequest(Database, _requestType, _account, status: RequestStatus.Approved);
            var dto = new RejectRequestDto { Id = request.Id, Reason = "Too late", IsFinal = false };

            var result = await Mediator.Send(new RejectRequestCommand(dto));

            Assert.That(result.Error!.ErrorCode, Is.EqualTo(400));
        }

        [Test]
        public async Task Handle_ForRevisionRequest_ReturnsFailure()
        {
            // Only Submited/Resubmited may be rejected per the handler's guard clause
            var request = DBSeeder.SeedRequest(Database, _requestType, _account, status: RequestStatus.ForRevision);
            var dto = new RejectRequestDto { Id = request.Id, Reason = "Already in revision", IsFinal = false };

            var result = await Mediator.Send(new RejectRequestCommand(dto));

            Assert.That(result.IsSuccess, Is.False);
        }
    }

    // ═══════════════════════════════════════════════════════
    //  GET ALL REQUESTS
    // ═══════════════════════════════════════════════════════

    [TestFixture]
    public class GetAllRequestsTests : HandlerTestBase
    {
        private RequestType _requestType = default!;
        private Account _account = default!;

        protected override async Task SeedAsync(AppDbContext db)
        {
            var region = DBSeeder.SeedRegion(db);
            _account = DBSeeder.SeedAccount(db, region);
            _requestType = DBSeeder.SeedRequestType(db, name: "IT Products");

            DBSeeder.SeedRequest(db, _requestType, _account, title: "Request A", status: RequestStatus.Submited);
            DBSeeder.SeedRequest(db, _requestType, _account, title: "Request B", status: RequestStatus.Approved);
        }

        [Test]
        public async Task Handle_ReturnsSuccess()
        {
            var result = await Mediator.Send(new GetAllRequestsCommand());

            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public async Task Handle_ReturnsAllSeededRequests()
        {
            var result = await Mediator.Send(new GetAllRequestsCommand());

            Assert.That(result.Data, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task Handle_MapsFieldsCorrectly()
        {
            var result = await Mediator.Send(new GetAllRequestsCommand());

            var dto = result.Data!.First(r => r.Title == "Request A");
            Assert.That(dto.Status, Is.EqualTo(RequestStatus.Submited.ToString()));
            Assert.That(dto.RequestType.Name, Is.EqualTo("IT Products"));
            Assert.That(dto.RequesterId, Is.EqualTo(_account.Id));
        }

        [Test]
        public async Task Handle_IncludesRequestsAcrossDifferentStatuses()
        {
            var result = await Mediator.Send(new GetAllRequestsCommand());

            Assert.That(result.Data!.Select(r => r.Status), Is.EquivalentTo(new[]
            {
                RequestStatus.Submited.ToString(),
                RequestStatus.Approved.ToString()
            }));
        }

        [Test]
        public async Task Handle_WhenNoRequests_ReturnsEmptyList()
        {
            await Database.Requests.ExecuteDeleteAsync();

            var result = await Mediator.Send(new GetAllRequestsCommand());

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data, Is.Empty);
        }
    }
}