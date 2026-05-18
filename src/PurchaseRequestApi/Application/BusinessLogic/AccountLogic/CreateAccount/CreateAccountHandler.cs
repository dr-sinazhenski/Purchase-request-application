using Application.BusinessLogic.AccountLogic.Dto;
using Infrastructure.Database;
using Infrastructure.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.AccountLogic.CreateAccount
{
    public class CreateAccountHandler : IRequestHandler<CreateAccountCommand, Result<AccountResDto>>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<CreateAccountHandler> _logger;

        public CreateAccountHandler(AppDbContext dbContext, ILogger<CreateAccountHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<AccountResDto>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating account with Login={Login}", request.Dto.Login);

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

            var account = new Account
            {
                Login = request.Dto.Login,
                Password = request.Dto.Password,
                Name = request.Dto.Name,
                RegionId = request.Dto.RegionId,
                ApproverProfileId = request.Dto.ApproverProfileId,
                Region = region,
                ApproverProfile = null!,
                Role = roles
            };

            await _dbContext.Accounts.AddAsync(account);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Account created with Id={Id}", account.Id);
            return Result<AccountResDto>.Success(new AccountResDto
            {
                Id = account.Id,
                Login = account.Login,
                Name = account.Name,
                RegionId = account.RegionId,
                RegionName = region.Name,
                ApproverProfileId = account.ApproverProfileId,
                RoleIds = roles.Select(r => r.Id).ToList(),
                RoleNames = roles.Select(r => r.Name).ToList()
            });
        }
    }
}