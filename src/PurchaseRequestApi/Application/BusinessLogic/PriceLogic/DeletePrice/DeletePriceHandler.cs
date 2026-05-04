using Infrastructure.Database;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.PriceLogic.DeletePrice
{
    public class DeletePriceHandler : IRequestHandler<DeletePriceCommand, Result<bool>>
    {
        private readonly ILogger<DeletePriceHandler> _logger;
        private readonly AppDbContext _dbContext;

        public DeletePriceHandler(AppDbContext dbContext, ILogger<DeletePriceHandler> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<Result<bool>> Handle(DeletePriceCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting price for ProductId {ProductId}, RegionId {RegionId}",
                command.ProductId, command.RegionId);

            var price = _dbContext.Prices.FirstOrDefault(x =>
                x.ProductId == command.ProductId && x.RegionId == command.RegionId);

            if (price == null)
                return null;

            _dbContext.Prices.Remove(price);
            await _dbContext.SaveChangesAsync();

            return Result<bool>.Success(true);
        }
    }
}