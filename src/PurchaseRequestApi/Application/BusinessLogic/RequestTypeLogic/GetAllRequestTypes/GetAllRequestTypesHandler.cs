using Application.BusinessLogic.RequestTypeLogic.Dto;
using Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.RequestTypeLogic.GetAllRequestTypes
{
    public class GetAllRequestTypesHandler : IRequestHandler<GetAllRequestTypesCommand, Result<List<RequestTypeResDto>>>
    {
        private readonly ILogger<GetAllRequestTypesHandler> _logger;
        private readonly AppDbContext _dbContext;

        public GetAllRequestTypesHandler(AppDbContext dbContext, ILogger<GetAllRequestTypesHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<List<RequestTypeResDto>>> Handle(GetAllRequestTypesCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching all request types");

            var requestTypes = await _dbContext.RequestTypes
                .AsNoTracking()
                .Select(rt => new RequestTypeResDto
                {
                    Id = rt.Id,
                    Name = rt.Name
                })
                .ToListAsync(cancellationToken);

            return Result<List<RequestTypeResDto>>.Success(requestTypes);
        }
    }
}