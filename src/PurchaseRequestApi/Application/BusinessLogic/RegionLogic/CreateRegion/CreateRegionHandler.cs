using Application.BusinessLogic.RegionLogic.Dto;
using Infrastructure.Database;
using Infrastructure.Database.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.RegionLogic.CreateRegion
{
    public class CreateRegionHandler : IRequestHandler<CreateRegionCommand, Result<CrudRegionDto>>
    {
        private readonly ILogger<CreateRegionHandler> _logger;
        private readonly AppDbContext _dbContext;

        public CreateRegionHandler(AppDbContext dbContext, ILogger<CreateRegionHandler> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<Result<CrudRegionDto>> Handle(CreateRegionCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating new region");

            var region = new Region
            {
                Id = Guid.NewGuid(),
                Name = command.dto.Name,
                Currency = command.dto.Currency,
                Accounts = new List<Account>(),
                Prices = new List<Price>()
            };

            await _dbContext.Regions.AddAsync(region);
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