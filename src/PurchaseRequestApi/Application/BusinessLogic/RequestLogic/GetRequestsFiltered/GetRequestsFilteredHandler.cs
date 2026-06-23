using Application.BusinessLogic.RequestLogic.Dto;
using Infrastructure.Database;
using Infrastructure.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.RequestLogic.GetRequestsFiltered
{
    public class GetRequestsFilteredHandler
        : IRequestHandler<GetRequestsFilteredCommand, Result<List<GetRequestsFilteredDto>>>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<GetRequestsFilteredHandler> _logger;

        public GetRequestsFilteredHandler(AppDbContext dbContext, ILogger<GetRequestsFilteredHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<List<GetRequestsFilteredDto>>> Handle(
            GetRequestsFilteredCommand request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Fetching requests filtered by RequestTypeId={RequestTypeId}, Status={Status}",
                request.RequestTypeId, request.Status);

            var query = _dbContext.Requests
                .AsNoTracking()
                .AsQueryable();

            if (request.RequestTypeId.HasValue)
                query = query.Where(r => r.RequestTypeId == request.RequestTypeId.Value);

            if (!string.IsNullOrWhiteSpace(request.Status) &&
                Enum.TryParse<RequestStatus>(request.Status, ignoreCase: true, out var parsedStatus))
                query = query.Where(r => r.Status == parsedStatus);

            var requests = await query
                .Select(r => new GetRequestsFilteredDto
                {
                    Id = r.Id,
                    Title = r.Title,
                    RequestType = r.RequestType.Name,
                    Status = r.Status.ToString(),
                    TotalPrice = request.RegionId.HasValue
                        ? r.RequesterProducts
                            .Sum(rp => rp.Product.Prices
                                .Where(p => p.RegionId == request.RegionId.Value)
                                .Select(p => p.Amount)
                                .FirstOrDefault() * rp.Quantity)
                        : 0,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    RequesterId = (Guid)r.RequesterId
                })
                .ToListAsync(cancellationToken);

            return Result<List<GetRequestsFilteredDto>>.Success(requests);
        }
    }
}