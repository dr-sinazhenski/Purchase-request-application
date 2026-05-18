using Application.BusinessLogic.ApproverProfileLogic.Dto;
using Infrastructure.Database;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.ApproverProfileLogic.UpdateApproverProfile
{
    public class UpdateApproverProfileHandler : IRequestHandler<UpdateApproverProfileCommand, Result<ApproverProfileResDto>>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<UpdateApproverProfileHandler> _logger;

        public UpdateApproverProfileHandler(AppDbContext dbContext, ILogger<UpdateApproverProfileHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<ApproverProfileResDto>> Handle(UpdateApproverProfileCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating approver profile with Id={Id}", request.Id);

            var profile = _dbContext.ApproverProfiles.FirstOrDefault(p => p.Id == request.Id);
            if (profile is null)
            {
                var err = new Error(404, $"ApproverProfile with id={request.Id} not found");
                _logger.LogError(err.ToString());
                return Result<ApproverProfileResDto>.Failure(err);
            }

            if (request.Dto.MinAmount >= request.Dto.MaxAmount)
            {
                var err = new Error(400, "MinAmount must be less than MaxAmount");
                _logger.LogError(err.ToString());
                return Result<ApproverProfileResDto>.Failure(err);
            }

            profile.Name = request.Dto.Name;
            profile.MinAmount = request.Dto.MinAmount;
            profile.MaxAmount = request.Dto.MaxAmount;
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("ApproverProfile updated with Id={Id}", profile.Id);
            return Result<ApproverProfileResDto>.Success(new ApproverProfileResDto
            {
                Id = profile.Id,
                Name = profile.Name,
                MinAmount = profile.MinAmount,
                MaxAmount = profile.MaxAmount
            });
        }
    }
}