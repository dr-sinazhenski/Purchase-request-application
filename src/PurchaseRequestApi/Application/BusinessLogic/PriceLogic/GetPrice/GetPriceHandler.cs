using Application.BusinessLogic.PriceLogic.Dto;
using Infrastructure.CurrencyRatesService;
using Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.PriceLogic.GetPrice
{
    public class GetPriceHandler : IRequestHandler<GetPriceCommand, Result<CrudPriceDto>>
    {
        private readonly ILogger<GetPriceHandler> _logger;
        private readonly AppDbContext _dbContext;
        private readonly CurrencyExchangeService _currencyExchangeService;

        public GetPriceHandler(AppDbContext dbContext, ILogger<GetPriceHandler> logger, CurrencyExchangeService currencyExchangeService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _currencyExchangeService = currencyExchangeService;
        }

        public async Task<Result<CrudPriceDto>> Handle(GetPriceCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching price for ProductId {ProductId}, RegionId {RegionId}",
                command.ProductId, command.RegionId);

            var price = _dbContext.Prices.FirstOrDefault(x =>
                x.ProductId == command.ProductId && x.RegionId == command.RegionId);

            if (price == null)
            {
                 var err = new Error(404, $"Price with productId= {command.ProductId} and regionId= {command.RegionId} not found");
                _logger.LogError(err.ToString());
                return Result<CrudPriceDto>.Failure(err);
            }

            if (command.RequiredCurrency != string.Empty)
            {
                var originalCurrency = _dbContext.Regions.FirstOrDefault(x => x.Id == price.RegionId).Currency;

                if (originalCurrency != command.RequiredCurrency)
                {
                    var rate = await _currencyExchangeService.GetRateAsync(originalCurrency, command.RequiredCurrency);
                    price.Amount *= rate;
                }
            }

            return Result<CrudPriceDto>.Success(new CrudPriceDto
            {
                ProductId = price.ProductId,
                RegionId = price.RegionId,
                Amount = price.Amount,
                UnitsOfMeasure = price.UnitsOfMeasure
            });
        }
    }
}