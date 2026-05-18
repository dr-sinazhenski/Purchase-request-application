using Infrastructure.Database;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.ApproverProfileLogic.DeleteApproverProfile
{
    public class DeleteApproverProfileHandler : IRequestHandler<DeleteApproverProfileCommand, Result<bool>>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<DeleteApproverProfileHandler> _logger;

        public DeleteApproverProfileHandler(AppDbContext dbContext, ILogger<DeleteApproverProfileHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(DeleteApproverProfileCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting approver profile with Id={Id}", request.Id);

            var role = _dbContext.Roles.FirstOrDefault(r => r.Id == request.Id);
            if (role is null)
            {
                var err = new Error(404, $"Approver profile with id={request.Id} not found");
                _logger.LogError(err.ToString());
                return Result<bool>.Failure(err);
            }

            _dbContext.Roles.Remove(role);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Approver profile deleted with Id={Id}", request.Id);
            return Result<bool>.Success(true);
        }
    }
}