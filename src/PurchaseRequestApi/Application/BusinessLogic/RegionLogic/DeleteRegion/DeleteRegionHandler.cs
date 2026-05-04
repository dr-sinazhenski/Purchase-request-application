using Infrastructure.Database;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.RegionLogic.DeleteRegion
{
    public class DeleteRegionHandler : IRequestHandler<DeleteRegionCommand, Result<bool>>
    {
        private readonly ILogger<DeleteRegionHandler> _logger;
        private readonly AppDbContext _dbContext;

        public DeleteRegionHandler(AppDbContext dbContext, ILogger<DeleteRegionHandler> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<Result<bool>> Handle(DeleteRegionCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting region {Id}", command.Id);

            var region = _dbContext.Regions.FirstOrDefault(x => x.Id == command.Id);

            if (region == null)
                return null;

            _dbContext.Regions.Remove(region);
            await _dbContext.SaveChangesAsync();

            return Result<bool>.Success(true);
        }
    }
}