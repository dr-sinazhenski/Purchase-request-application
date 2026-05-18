using Application.BusinessLogic.ApproverProfileLogic.Dto;
using Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.ApproverProfileLogic.GetAllApproverProfiles
{
    public class GetAllApproverProfilesHandler : IRequestHandler<GetAllApproverProfilesCommand, Result<List<ApproverProfileResDto>>>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<GetAllApproverProfilesHandler> _logger;

        public GetAllApproverProfilesHandler(AppDbContext dbContext, ILogger<GetAllApproverProfilesHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<List<ApproverProfileResDto>>> Handle(GetAllApproverProfilesCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching all approver profiles");

            var profiles = await _dbContext.ApproverProfiles
                .AsNoTracking()
                .Select(p => new ApproverProfileResDto { Id = p.Id, Name = p.Name, MinAmount = p.MinAmount, MaxAmount = p.MaxAmount })
                .ToListAsync(cancellationToken);

            return Result<List<ApproverProfileResDto>>.Success(profiles);
        }
    }
}