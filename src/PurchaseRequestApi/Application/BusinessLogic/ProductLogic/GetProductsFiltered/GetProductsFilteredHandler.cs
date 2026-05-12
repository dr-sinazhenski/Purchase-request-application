using Application.BusinessLogic.ProductLogic.Dto;
using Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.ProductLogic.GetProductsFiltered
{
    public class GetProductsFilteredHandler 
        : IRequestHandler<GetProductsFilteredCommand, Result<List<GetFilteredProductDto>>>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<GetProductsFilteredHandler> _logger;

        public GetProductsFilteredHandler(
            AppDbContext dbContext,
            ILogger<GetProductsFilteredHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<List<GetFilteredProductDto>>> Handle(
            GetProductsFilteredCommand request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Fetching products filtered by RegionId={RegionId}, RequestTypeId={RequestTypeId}",
                request.RegionId, request.RequestTypeId);

            var query = _dbContext.Products
                .AsNoTracking()
                .AsQueryable();

            if (request.RequestTypeId.HasValue)
                query = query.Where(p =>
                    p.RequestType.Any(rt => rt.Id == request.RequestTypeId.Value));

            if (request.RegionId.HasValue)
                query = query.Where(p =>
                    p.Prices.Any(pr => pr.RegionId == request.RegionId.Value));

            var products = await query
                .Select(p => new GetFilteredProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    RequestTypeIds = p.RequestType.Select(rt => rt.Id).ToList(),
                    Currency = p.Prices
                        .Where(pr => pr.RegionId == request.RegionId.Value)
                        .Select(pr => pr.Region.Currency)
                        .FirstOrDefault(),
                    Amount = p.Prices
                        .Where(pr => pr.RegionId == request.RegionId.Value)
                        .Select(pr => pr.Amount)
                        .Cast<decimal?>()
                        .FirstOrDefault(),
                    UnitsOfMeasure = p.Prices
                        .Where(pr => pr.RegionId == request.RegionId.Value)
                        .Select(pr => pr.UnitsOfMeasure)
                        .FirstOrDefault()
                })
                .ToListAsync(cancellationToken);

            return Result<List<GetFilteredProductDto>>.Success(products);
        }
    }
}