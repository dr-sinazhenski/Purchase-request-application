using Application.BusinessLogic.PriceLogic.Dto;
using Infrastructure.Database;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.PriceLogic.GetAllPrices
{
    public class GetAllPricesHandler : IRequestHandler<GetAllPricesCommand, Result<List<CrudPriceDto>>>
    {
        private readonly ILogger<GetAllPricesHandler> _logger;
        private readonly AppDbContext _dbContext;

        public GetAllPricesHandler(AppDbContext dbContext, ILogger<GetAllPricesHandler> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
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

            return Result<List<CrudPriceDto>>.Success(prices);
        }
    }
}