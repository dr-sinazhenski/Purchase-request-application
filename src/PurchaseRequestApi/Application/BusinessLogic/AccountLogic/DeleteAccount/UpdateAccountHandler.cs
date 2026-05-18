using Application.BusinessLogic.AccountLogic.Dto;
using Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.AccountLogic.UpdateAccount
{
    public class UpdateAccountHandler : IRequestHandler<UpdateAccountCommand, Result<AccountResDto>>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<UpdateAccountHandler> _logger;

        public UpdateAccountHandler(AppDbContext dbContext, ILogger<UpdateAccountHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<AccountResDto>> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating account with Id={Id}", request.Dto.Id);

            var account = await _dbContext.Accounts
                .Include(a => a.Role)
                .Include(a => a.ApproverProfile)
                .FirstOrDefaultAsync(a => a.Id == request.Dto.Id, cancellationToken);

            if (account is null)
            {
                var err = new Error(404, $"Account with id={request.Dto.Id} not found");
                _logger.LogError(err.ToString());
                return Result<AccountResDto>.Failure(err);
            }

            var region = _dbContext.Regions.FirstOrDefault(r => r.Id == request.Dto.RegionId);
            if (region is null)
            {
                var err = new Error(404, $"Region with id={request.Dto.RegionId} not found");
                _logger.LogError(err.ToString());
                return Result<AccountResDto>.Failure(err);
            }

            if (request.Dto.ApproverProfileId.HasValue)
            {
                var profileExists = _dbContext.ApproverProfiles.Any(p => p.Id == request.Dto.ApproverProfileId.Value);
                if (!profileExists)
                {
                    var err = new Error(404, $"ApproverProfile with id={request.Dto.ApproverProfileId} not found");
                    _logger.LogError(err.ToString());
                    return Result<AccountResDto>.Failure(err);
                }
            }

            var roles = await _dbContext.Roles
                .Where(r => request.Dto.RoleIds.Contains(r.Id))
                .ToListAsync(cancellationToken);

            account.Login = request.Dto.Login;
            account.Password = request.Dto.Password;
            account.Name = request.Dto.Name;
            account.RegionId = request.Dto.RegionId;
            account.ApproverProfileId = request.Dto.ApproverProfileId;
            account.Role = roles;

            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Account updated with Id={Id}", account.Id);
            return Result<AccountResDto>.Success(new AccountResDto
            {
                Id = account.Id,
                Login = account.Login,
                Name = account.Name,
                RegionId = account.RegionId,
                RegionName = region.Name,
                ApproverProfileId = account.ApproverProfileId,
                ApproverProfileName = account.ApproverProfile?.Name,
                RoleIds = roles.Select(r => r.Id).ToList(),
                RoleNames = roles.Select(r => r.Name).ToList()
            });
        }
    }
}