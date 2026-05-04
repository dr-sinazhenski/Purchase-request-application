using Application.BusinessLogic.PriceLogic.Dto;
using Infrastructure.Database;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.PriceLogic.GetPrice
{
    public class GetPriceHandler : IRequestHandler<GetPriceCommand, Result<CrudPriceDto>>
    {
        private readonly ILogger<GetPriceHandler> _logger;
        private readonly AppDbContext _dbContext;

        public GetPriceHandler(AppDbContext dbContext, ILogger<GetPriceHandler> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<Result<CrudPriceDto>> Handle(GetPriceCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching price for ProductId {ProductId}, RegionId {RegionId}",
                command.ProductId, command.RegionId);

            var price = _dbContext.Prices.FirstOrDefault(x =>
                x.ProductId == command.ProductId && x.RegionId == command.RegionId);

            if (price == null)
                return null;

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