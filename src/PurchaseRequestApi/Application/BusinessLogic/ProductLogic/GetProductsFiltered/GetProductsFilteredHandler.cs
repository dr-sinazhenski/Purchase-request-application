using Application.BusinessLogic.ProductLogic.Dto;
using Infrastructure.CurrencyRatesService;
using Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Application.BusinessLogic.ProductLogic.GetProductsFiltered
{
    public class GetProductsFilteredHandler 
        : IRequestHandler<GetProductsFilteredCommand, Result<List<GetFilteredProductDto>>>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<GetProductsFilteredHandler> _logger;
        private readonly CurrencyExchangeService _currencyExchangeService;

        public GetProductsFilteredHandler(
            AppDbContext dbContext,
            ILogger<GetProductsFilteredHandler> logger,
            CurrencyExchangeService currencyExchangeService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _currencyExchangeService = currencyExchangeService;
        }

        public async Task<Result<List<GetFilteredProductDto>>> Handle(
            GetProductsFilteredCommand request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Fetching products filtered by RegionId={RegionId}, RequestTypeId={RequestTypeId}",
                request.RegionId, request.RequestTypeId);

            var query = _dbContext.Products
                .AsNoTracking()
                .AsQueryable();

            if (request.RequestTypeId.HasValue)
                query = query.Where(p =>
                    p.RequestType.Any(rt => rt.Id == request.RequestTypeId.Value));

            if (request.RegionId.HasValue)
                query = query.Where(p =>
                    p.Prices.Any(pr => pr.RegionId == request.RegionId.Value));

            var products = await query
                .Select(p => new GetFilteredProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    RequestTypeIds = p.RequestType.Select(rt => rt.Id).ToList(),
                    Currency = p.Prices
                        .Where(pr => pr.RegionId == request.RegionId.Value)
                        .Select(pr => pr.Region.Currency)
                        .FirstOrDefault(),
                    Amount = p.Prices
                        .Where(pr => pr.RegionId == request.RegionId.Value)
                        .Select(pr => pr.Amount)
                        .Cast<decimal?>()
                        .FirstOrDefault(),
                    UnitsOfMeasure = p.Prices
                        .Where(pr => pr.RegionId == request.RegionId.Value)
                        .Select(pr => pr.UnitsOfMeasure)
                        .FirstOrDefault()
                })
                .ToListAsync(cancellationToken);

            if (request.RequiredCurrency != string.Empty)
            {
                for (var i = 0; i < products.Count; i++)
                {
                    if (products[i].Currency != request.RequiredCurrency)
                    {
                        var rate = await _currencyExchangeService.GetRateAsync(products[i].Currency, request.RequiredCurrency);
                        products[i].Amount *= rate;
                        products[i].Currency = request.RequiredCurrency;
                    }
                }
            }
            return Result<List<GetFilteredProductDto>>.Success(products);
        }
    }
}