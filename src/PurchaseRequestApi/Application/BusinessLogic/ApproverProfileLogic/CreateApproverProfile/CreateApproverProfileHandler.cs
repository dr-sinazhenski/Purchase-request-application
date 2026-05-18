using Application.BusinessLogic.ApproverProfileLogic.Dto;
using Infrastructure.Database;
using Infrastructure.Database.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.ApproverProfileLogic.CreateApproverProfile
{
    public class CreateApproverProfileHandler : IRequestHandler<CreateApproverProfileCommand, Result<ApproverProfileResDto>>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<CreateApproverProfileHandler> _logger;

        public CreateApproverProfileHandler(AppDbContext dbContext, ILogger<CreateApproverProfileHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<ApproverProfileResDto>> Handle(CreateApproverProfileCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating approver profile with Name={Name}", request.Dto.Name);

            if (request.Dto.MinAmount >= request.Dto.MaxAmount)
            {
                var err = new Error(400, "MinAmount must be less than MaxAmount");
                _logger.LogError(err.ToString());
                return Result<ApproverProfileResDto>.Failure(err);
            }

            var profile = new ApproverProfile
            {
                Name = request.Dto.Name,
                MinAmount = request.Dto.MinAmount,
                MaxAmount = request.Dto.MaxAmount,
                Accounts = new List<Account>()
            };

            await _dbContext.ApproverProfiles.AddAsync(profile);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("ApproverProfile created with Id={Id}", profile.Id);
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