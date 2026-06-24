using Application.BusinessLogic.PriceLogic.Dto;
using Infrastructure.CurrencyRatesService;
using Infrastructure.Database;
using Infrastructure.Database.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.PriceLogic.GetAllPrices
{
    public class GetAllPricesHandler : IRequestHandler<GetAllPricesCommand, Result<List<CrudPriceDto>>>
    {
        private readonly ILogger<GetAllPricesHandler> _logger;
        private readonly AppDbContext _dbContext;
        private readonly ICurrencyExchangeService _currencyExchangeService;

        public GetAllPricesHandler(AppDbContext dbContext, ILogger<GetAllPricesHandler> logger, ICurrencyExchangeService currencyExchangeService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _currencyExchangeService = currencyExchangeService;
        }

        public async Task<Result<List<CrudPriceDto>>> Handle(GetAllPricesCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching all prices");
            var prices = _dbContext.Prices.Select(p => new CrudPriceDto
            {
                ProductId = p.ProductId,
                RegionId = p.RegionId,
                Amount = p.Amount,
                UnitsOfMeasure = p.UnitsOfMeasure
            }).ToList();

            if (command.RequiredCurrency != string.Empty)
            {
                for (var i = 0; i < prices.Count; i++)
                {
                    var originalCurrency = _dbContext.Regions.FirstOrDefault(x => x.Id == prices[i].RegionId).Currency;

                    if (originalCurrency != command.RequiredCurrency)
                    {
                        var rate = await _currencyExchangeService.GetRateAsync(originalCurrency, command.RequiredCurrency);
                        prices[i].Amount *= rate;
                    }
                }
            }

            return Result<List<CrudPriceDto>>.Success(prices);
        }
    }
}