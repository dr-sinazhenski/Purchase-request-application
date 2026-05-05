using Application.BusinessLogic.RequestLogic.Dto;
using Infrastructure.Database;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.PriceLogic.DeletePrice
{
    public class DeletePriceHandler : IRequestHandler<DeletePriceCommand, Result>
    {
        private readonly ILogger<DeletePriceHandler> _logger;
        private readonly AppDbContext _dbContext;

        public DeletePriceHandler(AppDbContext dbContext, ILogger<DeletePriceHandler> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<Result> Handle(DeletePriceCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting price for ProductId {ProductId}, RegionId {RegionId}",
                command.ProductId, command.RegionId);

            var price = _dbContext.Prices.FirstOrDefault(x =>
                x.ProductId == command.ProductId && x.RegionId == command.RegionId);

            if (price == null)
            {
                var err = new Error(404, $"Price with productId= {command.ProductId} and regionId= {command.RegionId} not found");
                _logger.LogError(err.ToString());
                return Result<GetRequestDetailsResDto>.Failure(err);
            }

            _dbContext.Prices.Remove(price);
            await _dbContext.SaveChangesAsync();

            return Result.Success();
        }
    }
}