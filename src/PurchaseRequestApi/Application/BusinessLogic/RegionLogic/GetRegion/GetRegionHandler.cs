using Application.BusinessLogic.RegionLogic.Dto;
using Infrastructure.Database;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.RegionLogic.GetRegion
{
    public class GetRegionHandler : IRequestHandler<GetRegionCommand, Result<CrudRegionDto>>
    {
        private readonly ILogger<GetRegionHandler> _logger;
        private readonly AppDbContext _dbContext;

        public GetRegionHandler(AppDbContext dbContext, ILogger<GetRegionHandler> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<Result<CrudRegionDto>> Handle(GetRegionCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching region {Id}", command.Id);

            var region = _dbContext.Regions.FirstOrDefault(x => x.Id == command.Id);

            if (region == null)
            {
                var err = new Error(404, $"Region with id= {command.Id} not found");
                _logger.LogError(err.ToString());
                return Result<CrudRegionDto>.Failure(err);
            }

            return Result<CrudRegionDto>.Success(new CrudRegionDto
            {
                Id = region.Id,
                Name = region.Name,
                Currency = region.Currency
            });
        }
    }
}