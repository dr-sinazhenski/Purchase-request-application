using Application.BusinessLogic.RegionLogic.Dto;
using Infrastructure.Database;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.RegionLogic.GetAllRegions
{
    public class GetAllRegionsHandler : IRequestHandler<GetAllRegionsCommand, Result<List<CrudRegionDto>>>
    {
        private readonly ILogger<GetAllRegionsHandler> _logger;
        private readonly AppDbContext _dbContext;

        public GetAllRegionsHandler(AppDbContext dbContext, ILogger<GetAllRegionsHandler> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<Result<List<CrudRegionDto>>> Handle(GetAllRegionsCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching all regions");

            var regions = _dbContext.Regions.Select(r => new CrudRegionDto
            {
                Id = r.Id,
                Name = r.Name,
                Currency = r.Currency
            }).ToList();

            return Result<List<CrudRegionDto>>.Success(regions);
        }
    }
}