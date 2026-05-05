using Application.BusinessLogic.RequestLogic.Dto;
using Infrastructure.Database;
using Infrastructure.Database.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.RegionLogic.DeleteRegion
{
    public class DeleteRegionHandler : IRequestHandler<DeleteRegionCommand, Result>
    {
        private readonly ILogger<DeleteRegionHandler> _logger;
        private readonly AppDbContext _dbContext;

        public DeleteRegionHandler(AppDbContext dbContext, ILogger<DeleteRegionHandler> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<Result> Handle(DeleteRegionCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting region {Id}", command.Id);

            var region = _dbContext.Regions.FirstOrDefault(x => x.Id == command.Id);

            if (region == null)
            {
                var err = new Error(404, $"Region with id= {command.Id} not found");
                _logger.LogError(err.ToString());
                return Result.Failure(err);
            }

            _dbContext.Regions.Remove(region);
            await _dbContext.SaveChangesAsync();

            return Result.Success();
        }
    }
}