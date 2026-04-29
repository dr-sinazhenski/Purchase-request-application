using Application.BusinessLogic.RequestLogic.Dto;
using Application.BusinessLogic.RequestTypeLogic.Dto;
using Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.RequestLogic.GetAllRequests
{
    public class GetAllRequestsHandler : IRequestHandler<GetAllRequestsCommand, Result<List<GetAllRequestsDto>>>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<GetAllRequestsHandler> _logger;

        public GetAllRequestsHandler(AppDbContext dbContext, ILogger<GetAllRequestsHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<List<GetAllRequestsDto>>> Handle(GetAllRequestsCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching all requests");

            var requests = await _dbContext.Requests
                .AsNoTracking()
                .Include(r => r.RequestType)
                .Select(r => new GetAllRequestsDto
                {
                    Id = r.Id,
                    Title = r.Title,
                   /* Status = r.Status.ToString(),*/
                    CreatedAt = r.CreatedAt,
                    EditedAt = r.EditedAt,
                    RequestType = new RequestTypeResDto
                    {
                        Id = r.RequestType.Id,
                        Name = r.RequestType.Name
                    }
                })
                .ToListAsync(cancellationToken);

            return Result<List<GetAllRequestsDto>>.Success(requests);
        }
    }
}