using Infrastructure.Database;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.AccountLogic.DeleteAccount
{
    public class DeleteAccountHandler : IRequestHandler<DeleteAccountCommand, Result<bool>>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<DeleteAccountHandler> _logger;

        public DeleteAccountHandler(AppDbContext dbContext, ILogger<DeleteAccountHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting account with Id={Id}", request.Id);

            var account = _dbContext.Accounts.FirstOrDefault(a => a.Id == request.Id);
            if (account is null)
            {
                var err = new Error(404, $"Account with id={request.Id} not found");
                _logger.LogError(err.ToString());
                return Result<bool>.Failure(err);
            }

            _dbContext.Accounts.Remove(account);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Account deleted with Id={Id}", request.Id);
            return Result<bool>.Success(true);
        }
    }
}