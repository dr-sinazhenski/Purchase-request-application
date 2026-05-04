using Application.BusinessLogic.RegionLogic.Dto;
using Infrastructure.Database;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.RegionLogic.UpdateRegion
{
    public class UpdateRegionHandler : IRequestHandler<UpdateRegionCommand, Result<CrudRegionDto>>
    {
        private readonly ILogger<UpdateRegionHandler> _logger;
        private readonly AppDbContext _dbContext;

        public UpdateRegionHandler(AppDbContext dbContext, ILogger<UpdateRegionHandler> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<Result<CrudRegionDto>> Handle(UpdateRegionCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating region {Id}", command.dto.Id);

            var region = _dbContext.Regions.FirstOrDefault(x => x.Id == command.dto.Id);

            if (region == null)
                return null;

            region.Name = command.dto.Name;
            region.Currency = command.dto.Currency;

            await _dbContext.SaveChangesAsync();

            return Result<CrudRegionDto>.Success(new CrudRegionDto
            {
                Id = region.Id,
                Name = region.Name,
                Currency = region.Currency
            });
        }
    }
}