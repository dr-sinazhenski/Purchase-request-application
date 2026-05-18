using Application.BusinessLogic.AccountLogic.Dto;
using Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.AccountLogic.GetAllAccounts
{
    public class GetAllAccountsHandler : IRequestHandler<GetAllAccountsCommand, Result<List<AccountResDto>>>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<GetAllAccountsHandler> _logger;

        public GetAllAccountsHandler(AppDbContext dbContext, ILogger<GetAllAccountsHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<List<AccountResDto>>> Handle(GetAllAccountsCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching all accounts");

            var accounts = await _dbContext.Accounts
                .AsNoTracking()
                .Select(a => new AccountResDto
                {
                    Id = a.Id,
                    Login = a.Login,
                    Name = a.Name,
                    RegionId = a.RegionId,
                    RegionName = a.Region.Name,
                    ApproverProfileId = a.ApproverProfileId,
                    ApproverProfileName = a.ApproverProfile != null ? a.ApproverProfile.Name : null,
                    RoleIds = a.Role.Select(r => r.Id).ToList(),
                    RoleNames = a.Role.Select(r => r.Name).ToList()
                })
                .ToListAsync(cancellationToken);

            return Result<List<AccountResDto>>.Success(accounts);
        }
    }
}