using Application.BusinessLogic.PriceLogic.Dto;
using Infrastructure.Database;
using Infrastructure.Database.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.PriceLogic.CreatePrice
{
    public class CreatePriceHandler : IRequestHandler<CreatePriceCommand, Result<CrudPriceDto>>
    {
        private readonly ILogger<CreatePriceHandler> _logger;
        private readonly AppDbContext _dbContext;

        public CreatePriceHandler(AppDbContext dbContext, ILogger<CreatePriceHandler> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<Result<CrudPriceDto>> Handle(CreatePriceCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating new price for ProductId {ProductId}, RegionId {RegionId}",
                command.dto.ProductId, command.dto.RegionId);

            var product = _dbContext.Products.FirstOrDefault(x => x.Id == command.dto.ProductId);
            if (product == null)
                return null;

            var region = _dbContext.Regions.FirstOrDefault(x => x.Id == command.dto.RegionId);
            if (region == null)
                return null;

            var existing = _dbContext.Prices.FirstOrDefault(x =>
                x.ProductId == command.dto.ProductId && x.RegionId == command.dto.RegionId);
            if (existing != null)
                return null;

            var price = new Price
            {
                ProductId = command.dto.ProductId,
                RegionId = command.dto.RegionId,
                Amount = command.dto.Amount,
                UnitsOfMeasure = command.dto.UnitsOfMeasure,
                Product = product,
                Region = region
            };

            await _dbContext.Prices.AddAsync(price);
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