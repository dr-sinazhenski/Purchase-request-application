using Infrastructure.Database;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.RoleLogic.DeleteRole
{
    public class DeleteRoleHandler : IRequestHandler<DeleteRoleCommand, Result<bool>>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<DeleteRoleHandler> _logger;

        public DeleteRoleHandler(AppDbContext dbContext, ILogger<DeleteRoleHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting role with Id={Id}", request.Id);

            var role = _dbContext.Roles.FirstOrDefault(r => r.Id == request.Id);
            if (role is null)
            {
                var err = new Error(404, $"Role with id={request.Id} not found");
                _logger.LogError(err.ToString());
                return Result<bool>.Failure(err);
            }

            _dbContext.Roles.Remove(role);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Role deleted with Id={Id}", request.Id);
            return Result<bool>.Success(true);
        }
    }
}