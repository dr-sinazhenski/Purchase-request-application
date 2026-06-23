using Application.BusinessLogic.RequestLogic.Dto;
using Application.BusinessLogic.RequestTypeLogic.Dto;
using Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.RequestLogic.GetAllRequests
{
    public class GetAllRequestsHandler : IRequestHandler<GetAllRequestsCommand, Result<List<GetRequestResDto>>>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<GetAllRequestsHandler> _logger;

        public GetAllRequestsHandler(AppDbContext dbContext, ILogger<GetAllRequestsHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<List<GetRequestResDto>>> Handle(GetAllRequestsCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching all requests");

            var requests = await _dbContext.Requests
                .AsNoTracking()
                .Include(r => r.RequestType)
                .Select(r => new GetRequestResDto
                {
                    Id = r.Id,
                    Title = r.Title,
                    Status = r.Status.ToString(),
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    RequestType = new RequestTypeResDto
                    {
                        Id = r.RequestType.Id,
                        Name = r.RequestType.Name
                    },
                    RequesterId = (Guid)r.RequesterId
                })
                .ToListAsync(cancellationToken);

            return Result<List<GetRequestResDto>>.Success(requests);
        }
    }
}