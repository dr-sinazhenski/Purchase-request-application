using Application.BusinessLogic.RequestLogic.Dto;
using Infrastructure.CurrencyRatesService;
using Infrastructure.Database;
using Infrastructure.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;
using System.Diagnostics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Application.BusinessLogic.RequestLogic.GetRequestsFiltered
{
    public class GetRequestsFilteredHandler
        : IRequestHandler<GetRequestsFilteredCommand, Result<List<GetRequestsFilteredDto>>>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<GetRequestsFilteredHandler> _logger;
        private readonly ICurrencyExchangeService _currencyExchangeService;

        public GetRequestsFilteredHandler(AppDbContext dbContext, ILogger<GetRequestsFilteredHandler> logger, ICurrencyExchangeService currencyExchangeService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _currencyExchangeService = currencyExchangeService;
        }

        public async Task<Result<List<GetRequestsFilteredDto>>> Handle(
            GetRequestsFilteredCommand request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Fetching requests filtered by RequestTypeId={RequestTypeId}, Status={Status}",
                request.RequestTypeId, request.Status);

            var query = _dbContext.Requests
                .AsNoTracking()
                .AsQueryable();

            if (request.RequestTypeId.HasValue)
                query = query.Where(r => r.RequestTypeId == request.RequestTypeId.Value);

            if (!string.IsNullOrWhiteSpace(request.Status) &&
                Enum.TryParse<RequestStatus>(request.Status, ignoreCase: true, out var parsedStatus))
                query = query.Where(r => r.Status == parsedStatus);

            var requests = await query.Include(r => r.Requester).Include(r => r.RequestType).Include(r => r.RequesterProducts).ToListAsync();

            GetRequestsFilteredDto resReq;
            List<GetRequestsFilteredDto> result = new List<GetRequestsFilteredDto>();
            foreach (var r in requests)
            {
                if (r.Requester == null)
                {
                    _logger.LogWarning("Skipping request {Id} because requester is not assigned", r.Id);
                    continue;
                }

                resReq = new GetRequestsFilteredDto
                {
                    Id = r.Id,
                    Title = r.Title,
                    RequestType = r.RequestType.Name,
                    Status = r.Status.ToString(),
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    RequesterId = (Guid)r.RequesterId,
                    TotalPrice = 0
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
                    resReq.TotalPrice += r.RequesterProducts.First(x => x.ProductId == product.Id).Quantity * prices.First(p => p.ProductId == product.Id).Amount;
                }

                result.Add(resReq);
            }

            return Result<List<GetRequestsFilteredDto>>.Success(result);
        }
    }
}
