using Application.BusinessLogic.RequestLogic.Dto;
using Application.BusinessLogic.RequestTypeLogic.Dto;
using Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Application.BusinessLogic.RequestLogic.GetRequestById
{
    public class GetRequestByIdHandler : IRequestHandler<GetRequestByIdCommand, Result<GetRequestDetailsResDto>>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<GetRequestByIdHandler> _logger;

        public GetRequestByIdHandler(AppDbContext dbContext, ILogger<GetRequestByIdHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<GetRequestDetailsResDto>> Handle(GetRequestByIdCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching request {Id}", request.Id);

            var r = await _dbContext.Requests
                .AsNoTracking()
                .Include(r => r.RequestType)
                .Include(r => r.RequesterProducts)
                    .ThenInclude(rp => rp.Product)
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

            if (r == null)
            {
                var err = new Error(404, $"Request with id= {request.Id} not found");
                _logger.LogError(err.ToString());
                return Result<GetRequestDetailsResDto>.Failure(err);
            }

            var data = new GetRequestDetailsResDto
            {
                Id = r.Id,
                Title = r.Title,
                Description = r.Description,
                Status = r.Status.ToString(),
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt,
                RequestType = new RequestTypeResDto
                {
                    Id = r.RequestType.Id,
                    Name = r.RequestType.Name
                },
                Products = new List<ProductListItemDto>(),
                RequesterId = (Guid)r.RequesterId
            };

            var productsIds = r.RequesterProducts.Select(x => x.ProductId).ToList();
            var products = _dbContext.Products.Where(p => productsIds.Contains(p.Id)).ToList();
            var prices = _dbContext.Prices.Where(p => p.RegionId == new Guid("aaaaaaaa-bbbb-bbbb-bbbb-bbbbbbbbbbbb") && productsIds.Contains(p.Product.Id)).ToList();

            foreach (var product in products)
            {
                data.Products.Add(new ProductListItemDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Amount = r.RequesterProducts.First(x => x.ProductId == product.Id).Quantity,
                    Price = prices.First(p => p.ProductId == product.Id).Amount
                });
            }

            return Result<GetRequestDetailsResDto>.Success(data);
        }
    }
}
