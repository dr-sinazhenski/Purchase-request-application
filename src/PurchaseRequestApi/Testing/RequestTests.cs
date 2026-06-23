using Application.BusinessLogic.RequestLogic.ApproveRequest;
using Application.BusinessLogic.RequestLogic.CreateRequest;
using Application.BusinessLogic.RequestLogic.DeleteRequest;
using Application.BusinessLogic.RequestLogic.Dto;
using Infrastructure.Database;
using Infrastructure.Database.Entities;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Testing;

namespace Testing.RequestTests
{
    [TestFixture]
    public class CreateRequestTests : HandlerTestBase
    {
        private RequestType _requestType = default!;
        private Region _region = default!;
        private Account _account = default!;
        private Product _product = default!;

        protected override async Task SeedAsync(AppDbContext db)
        {
            _region = DBSeeder.SeedRegion(db);
            _account = DBSeeder.SeedAccount(db, _region);
            _requestType = DBSeeder.SeedRequestType(db, name: "IT Products");
            _product = DBSeeder.SeedProduct(db, _requestType, name: "Laptop");
            DBSeeder.SeedNorthAmericaPrice(db, _product.Id, amount: 999m);
        }

        [Test]
        public async Task Handle_ValidDtoWithProducts_ReturnsSuccessWithCorrectData()
        {
            var dto = new CreateRequestDto
            {
                Title = "My Request",
                Description = "Need a laptop",
                RequestTypeId = _requestType.Id,
                RequesterId = _account.Id,
                ProductIdAmount = new Dictionary<Guid, int> { { _product.Id, 2 } }
            };

            var result = await Mediator.Send(new CreateRequestCommand(dto));

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data!.Title, Is.EqualTo("My Request"));
            Assert.That(result.Data.Status, Is.EqualTo(RequestStatus.Submited.ToString()));
            Assert.That(result.Data.Products, Has.Count.EqualTo(1));
            Assert.That(result.Data.Products[0].Amount, Is.EqualTo(2));
            Assert.That(result.Data.Products[0].Price, Is.EqualTo(999m));
        }

        [Test]
        public async Task Handle_ValidDtoWithProducts_PersistsRequestToDatabase()
        {
            var dto = new CreateRequestDto
            {
                Title = "Persisted Request",
                Description = "Should be in DB",
                RequestTypeId = _requestType.Id,
                RequesterId = _account.Id,
                ProductIdAmount = new Dictionary<Guid, int> { { _product.Id, 1 } }
            };

            await Mediator.Send(new CreateRequestCommand(dto));

            var count = await Database.Requests.CountAsync(r => r.Title == "Persisted Request");
            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public async Task Handle_ValidDtoWithProducts_PersistsRequesterProducts()
        {
            var dto = new CreateRequestDto
            {
                Title = "With Products",
                Description = "desc",
                RequestTypeId = _requestType.Id,
                RequesterId = _account.Id,
                ProductIdAmount = new Dictionary<Guid, int> { { _product.Id, 3 } }
            };

            var result = await Mediator.Send(new CreateRequestCommand(dto));

            var rp = await Database.RequesterProducts
                .FirstOrDefaultAsync(x => x.RequestId == result.Data!.Id && x.ProductId == _product.Id);

            Assert.That(rp, Is.Not.Null);
            Assert.That(rp!.Quantity, Is.EqualTo(3));
        }

        [Test]
        public async Task Handle_InvalidRequestTypeId_ReturnsFailure()
        {
            var dto = new CreateRequestDto
            {
                Title = "Bad Type",
                Description = "desc",
                RequestTypeId = Guid.NewGuid(), // non-existent
                ProductIdAmount = new Dictionary<Guid, int>()
            };

            var result = await Mediator.Send(new CreateRequestCommand(dto));

            Assert.That(result.IsSuccess, Is.False);
        }

        [Test]
        public async Task Handle_ProductNotBelongingToRequestType_IsNotIncluded()
        {
            // Product belonging to a different request type
            var otherType = DBSeeder.SeedRequestType(Database, name: "Office Supplies");
            var foreignProduct = DBSeeder.SeedProduct(Database, otherType, name: "Chair");
            DBSeeder.SeedNorthAmericaPrice(Database, foreignProduct.Id, 200m);

            var dto = new CreateRequestDto
            {
                Title = "Mixed Products",
                Description = "desc",
                RequestTypeId = _requestType.Id,
                RequesterId = _account.Id,
                ProductIdAmount = new Dictionary<Guid, int>
                {
                    { _product.Id, 1 },
                    { foreignProduct.Id, 1 } // belongs to different type, should be excluded
                }
            };

            var result = await Mediator.Send(new CreateRequestCommand(dto));

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data!.Products, Has.Count.EqualTo(1));
            Assert.That(result.Data.Products[0].Id, Is.EqualTo(_product.Id));
        }

        [Test]
        public async Task Handle_ZeroQuantityProduct_IsNotIncluded()
        {
            var dto = new CreateRequestDto
            {
                Title = "Zero Qty",
                Description = "desc",
                RequestTypeId = _requestType.Id,
                RequesterId = _account.Id,
                ProductIdAmount = new Dictionary<Guid, int> { { _product.Id, 0 } }
            };

            var result = await Mediator.Send(new CreateRequestCommand(dto));

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data!.Products, Is.Empty);

            var rpCount = await Database.RequesterProducts
                .CountAsync(rp => rp.ProductId == _product.Id);
            Assert.That(rpCount, Is.EqualTo(0));
        }

        [Test]
        public async Task Handle_EmptyProductIdAmount_CreatesRequestWithNoProducts()
        {
            var dto = new CreateRequestDto
            {
                Title = "No Products",
                Description = "desc",
                RequestTypeId = _requestType.Id,
                RequesterId = _account.Id,
                ProductIdAmount = new Dictionary<Guid, int>()
            };

            var result = await Mediator.Send(new CreateRequestCommand(dto));

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Data!.Products, Is.Empty);
        }

        [Test]
        public async Task Handle_NewRequest_HasSubmittedStatus()
        {
            var dto = new CreateRequestDto
            {
                Title = "Status Check",
                Description = "desc",
                RequestTypeId = _requestType.Id,
                RequesterId = _account.Id,
                ProductIdAmount = new Dictionary<Guid, int>()
            };

            var result = await Mediator.Send(new CreateRequestCommand(dto));

            Assert.That(result.Data!.Status, Is.EqualTo(RequestStatus.Submited.ToString()));
        }
    }

    [TestFixture]
    public class DeleteRequestTests : HandlerTestBase
    {
        private Request _request = default!;

        protected override async Task SeedAsync(AppDbContext db)
        {
            var region = DBSeeder.SeedRegion(db);
            var account = DBSeeder.SeedAccount(db, region);
            var requestType = DBSeeder.SeedRequestType(db);
            _request = DBSeeder.SeedRequest(db, requestType, account, title: "To Be Deleted");
        }

        [Test]
        public async Task Handle_ExistingRequest_ReturnsSuccess()
        {
            var result = await Mediator.Send(new DeleteRequestCommand(_request.Id));

            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public async Task Handle_ExistingRequest_RemovesFromDatabase()
        {
            await Mediator.Send(new DeleteRequestCommand(_request.Id));

            var exists = await Database.Requests.AnyAsync(r => r.Id == _request.Id);
            Assert.That(exists, Is.False);
        }

        [Test]
        public async Task Handle_NonExistentId_ReturnsFailure()
        {
            var result = await Mediator.Send(new DeleteRequestCommand(Guid.NewGuid()));

            Assert.That(result.IsSuccess, Is.False);
        }

        [Test]
        public async Task Handle_NonExistentId_Returns404Error()
        {
            var result = await Mediator.Send(new DeleteRequestCommand(Guid.NewGuid()));

            Assert.That(result.Error!.ErrorCode, Is.EqualTo(404));
        }
    }

    [TestFixture]
    public class ApproveRequestTests : HandlerTestBase
    {
        private Region _region = default!;
        private Account _account = default!;
        private RequestType _requestType = default!;

        protected override async Task SeedAsync(AppDbContext db)
        {
            _region = DBSeeder.SeedRegion(db);
            _account = DBSeeder.SeedAccount(db, _region);
            _requestType = DBSeeder.SeedRequestType(db);
        }

        [Test]
        public async Task Handle_SubmittedRequest_ReturnsSuccess()
        {
            var request = DBSeeder.SeedRequest(Database, _requestType, _account, status: RequestStatus.Submited);

            var result = await Mediator.Send(new ApproveRequestCommand(request.Id));

            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public async Task Handle_ResubmittedRequest_ReturnsSuccess()
        {
            var request = DBSeeder.SeedRequest(Database, _requestType, _account, status: RequestStatus.Resubmited);

            var result = await Mediator.Send(new ApproveRequestCommand(request.Id));

            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public async Task Handle_SubmittedRequest_ChangesStatusToApproved()
        {
            var request = DBSeeder.SeedRequest(Database, _requestType, _account, status: RequestStatus.Submited);

            await Mediator.Send(new ApproveRequestCommand(request.Id));

            var updated = await Database.Requests.FindAsync(request.Id);
            Assert.That(updated!.Status, Is.EqualTo(RequestStatus.Approved));
        }

        [Test]
        public async Task Handle_AlreadyApprovedRequest_ReturnsFailure()
        {
            var request = DBSeeder.SeedRequest(Database, _requestType, _account, status: RequestStatus.Approved);

            var result = await Mediator.Send(new ApproveRequestCommand(request.Id));

            Assert.That(result.IsSuccess, Is.False);
        }

        [Test]
        public async Task Handle_AlreadyApprovedRequest_Returns400Error()
        {
            var request = DBSeeder.SeedRequest(Database, _requestType, _account, status: RequestStatus.Approved);

            var result = await Mediator.Send(new ApproveRequestCommand(request.Id));

            Assert.That(result.Error!.ErrorCode, Is.EqualTo(400));
        }

        [Test]
        public async Task Handle_RejectedRequest_ReturnsFailure()
        {
            var request = DBSeeder.SeedRequest(Database, _requestType, _account, status: RequestStatus.ForRevision);

            var result = await Mediator.Send(new ApproveRequestCommand(request.Id));

            Assert.That(result.IsSuccess, Is.False);
        }

        [Test]
        public async Task Handle_NonExistentId_ReturnsFailure()
        {
            var result = await Mediator.Send(new ApproveRequestCommand(Guid.NewGuid()));

            Assert.That(result.IsSuccess, Is.False);
        }

        [Test]
        public async Task Handle_NonExistentId_Returns404Error()
        {
            var result = await Mediator.Send(new ApproveRequestCommand(Guid.NewGuid()));

            Assert.That(result.Error!.ErrorCode, Is.EqualTo(404));
        }
    

        [Test]
        public async Task Add_adds_request_successfully2()
        {

            var type = await Database.RequestTypes.FirstOrDefaultAsync();
            var dto = new CreateRequestDto
            {
                Title = "123",
                Description = "456",
                RequestTypeId = type.Id,
                RequesterId = _account.Id, 
            };

            var responce = await Mediator.Send(new CreateRequestCommand(dto));

            Assert.That(responce.IsSuccess, Is.EqualTo(true));

            var count = await Database.Requests.CountAsync(c => c.Title == dto.Title);
            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public async Task Add_adds_request_successfully3()
        {

            var type = await Database.RequestTypes.FirstOrDefaultAsync();
            var dto = new CreateRequestDto
            {
                Title = "123",
                Description = "456",
                RequestTypeId = type.Id,
                RequesterId = _account.Id, 
            };

            var responce = await Mediator.Send(new CreateRequestCommand(dto));

            Assert.That(responce.IsSuccess, Is.EqualTo(true));

            var count = await Database.Requests.CountAsync(c => c.Title == dto.Title);
            Assert.That(count, Is.EqualTo(1));
        }

    }
}
