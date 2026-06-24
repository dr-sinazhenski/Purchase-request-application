using Application.BusinessLogic.RequestLogic.Dto;
using Application.BusinessLogic.RequestTypeLogic.Dto;
using Infrastructure.CurrencyRatesService;
using Infrastructure.Database;
using Infrastructure.Database.Entities;
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
        private readonly ICurrencyExchangeService _currencyExchangeService;

        public GetRequestByIdHandler(AppDbContext dbContext, ILogger<GetRequestByIdHandler> logger, ICurrencyExchangeService currencyExchangeService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _currencyExchangeService = currencyExchangeService;
        }

        public async Task<Result<GetRequestDetailsResDto>> Handle(GetRequestByIdCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching request {Id}", request.Id);

            var r = await _dbContext.Requests
                .AsNoTracking()
                .Include(r => r.Requester)
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

            if (r.Requester == null)
            {
                var err = new Error(400, $"Request with id= {request.Id} has no requester assigned");
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
            var prices = _dbContext.Prices.Where(p => p.RegionId == r.Requester.RegionId && productsIds.Contains(p.Product.Id)).ToList();

            if (request.RequiredCurrency != string.Empty)
            {
                var originalCurrency = _dbContext.Regions.FirstOrDefault(x => x.Id == r.Requester.RegionId).Currency;
                if (originalCurrency != request.RequiredCurrency)
                {
                    var rate = await _currencyExchangeService.GetRateAsync(originalCurrency, request.RequiredCurrency);

                    for (var i = 0; i < prices.Count; i++)
                    {
                        prices[i].Amount *= rate;
                    }
                }
            }

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
