using Application.BusinessLogic.PriceLogic.Dto;
using Application.BusinessLogic.RequestLogic.Dto;
using Infrastructure.Database;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.PriceLogic.UpdatePrice
{
    public class UpdatePriceHandler : IRequestHandler<UpdatePriceCommand, Result<CrudPriceDto>>
    {
        private readonly ILogger<UpdatePriceHandler> _logger;
        private readonly AppDbContext _dbContext;

        public UpdatePriceHandler(AppDbContext dbContext, ILogger<UpdatePriceHandler> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<Result<CrudPriceDto>> Handle(UpdatePriceCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating price for ProductId {ProductId}, RegionId {RegionId}",
                command.dto.ProductId, command.dto.RegionId);

            var price = _dbContext.Prices.FirstOrDefault(x =>
                x.ProductId == command.dto.ProductId && x.RegionId == command.dto.RegionId);

            if (price == null)
            {
                var err = new Error(404, $"Price with productId= {command.dto.ProductId} and regionId= {command.dto.RegionId} not found");
                _logger.LogError(err.ToString());
                return Result<CrudPriceDto>.Failure(err);
            }

            price.Amount = command.dto.Amount;
            price.UnitsOfMeasure = command.dto.UnitsOfMeasure;

            await _dbContext.SaveChangesAsync();

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