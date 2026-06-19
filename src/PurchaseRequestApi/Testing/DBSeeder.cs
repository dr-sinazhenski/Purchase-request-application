using Infrastructure.Database;
using Infrastructure.Database.Entities;

namespace Testing
{
    public static class DBSeeder
    {
        public static RequestType SeedRequestType(AppDbContext db, Guid? id = null, string name = "Default Type")
        {
            var requestType = new RequestType
            {
                Id = id ?? Guid.NewGuid(),
                Name = name
            };

            db.RequestTypes.Add(requestType);
            db.SaveChanges();

            return requestType;
        }

        public static Region SeedRegion(AppDbContext db, Guid? id = null, string name = "Test Region", string currency = "USD")
        {
            var region = new Region
            {
                Id = id ?? Guid.NewGuid(),
                Name = name,
                Currency = currency
            };

            db.Regions.Add(region);
            db.SaveChanges();

            return region;
        }

        public static Account SeedAccount(AppDbContext db, Region region, Guid? id = null, string login = "test.user")
        {
            var account = new Account
            {
                Id = id ?? Guid.NewGuid(),
                Login = login,
                Password = "hashed_pw",
                Name = "Test User",
                RegionId = region.Id,
                Region = region,
                ApproverProfileId = null
            };

            db.Accounts.Add(account);
            db.SaveChanges();

            return account;
        }

        public static Product SeedProduct(AppDbContext db, RequestType requestType, Guid? id = null, string name = "Test Product")
        {
            var product = new Product
            {
                Id = id ?? Guid.NewGuid(),
                Name = name,
                Description = "Test description",
                RequestType = new List<RequestType> { requestType }
            };

            db.Products.Add(product);
            db.SaveChanges();

            return product;
        }

        /// <summary>
        /// Seeds a Price for the hardcoded North America region used in CreateRequestHandler.
        /// </summary>
        public static Price SeedNorthAmericaPrice(AppDbContext db, Guid productId, decimal amount = 100m)
        {
            // CreateRequestHandler hardcodes this region GUID for price lookup
            var northAmericaRegionId = Guid.Parse("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

            if (!db.Regions.Any(r => r.Id == northAmericaRegionId))
            {
                db.Regions.Add(new Region
                {
                    Id = northAmericaRegionId,
                    Name = "North America",
                    Currency = "USD"
                });
                db.SaveChanges();
            }

            var price = new Price
            {
                ProductId = productId,
                RegionId = northAmericaRegionId,
                Amount = amount,
                UnitsOfMeasure = "pcs"
            };

            db.Prices.Add(price);
            db.SaveChanges();

            return price;
        }

        public static Request SeedRequest(
            AppDbContext db,
            RequestType requestType,
            Account requester,
            Guid? id = null,
            string title = "Test Request",
            RequestStatus status = RequestStatus.Submited)
        {
            var request = new Request
            {
                Id = id ?? Guid.NewGuid(),
                Title = title,
                Description = "Test description",
                Status = status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                RequestTypeId = requestType.Id,
                RequestType = requestType,
                RequesterId = requester.Id,
                Requester = requester,
                RequesterProducts = new List<RequesterProduct>()
            };

            db.Requests.Add(request);
            db.SaveChanges();

            return request;
        }

        public static Comment SeedComment(
            AppDbContext db,
            Request request,
            Account? account = null,
            Guid? id = null,
            string text = "Test comment")
        {
            var comment = new Comment
            {
                Id = id ?? Guid.NewGuid(),
                RequestId = request.Id,
                Request = request,
                AccountId = account?.Id,
                Text = text,
                CreationTime = DateTime.UtcNow
            };

            db.Comments.Add(comment);
            db.SaveChanges();

            return comment;
        }
    }
}