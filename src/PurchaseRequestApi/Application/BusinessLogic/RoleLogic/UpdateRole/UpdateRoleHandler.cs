using Application.BusinessLogic.RoleLogic.Dto;
using Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.RoleLogic.UpdateRole
{
    public class UpdateRoleHandler : IRequestHandler<UpdateRoleCommand, Result<RoleResDto>>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<UpdateRoleHandler> _logger;

        public UpdateRoleHandler(AppDbContext dbContext, ILogger<UpdateRoleHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<RoleResDto>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating role with Id={Id}", request.Id);

            var role = _dbContext.Roles.FirstOrDefault(r => r.Id == request.Id);
            if (role is null)
            {
                var err = new Error(404, $"Role with id={request.Id} not found");
                _logger.LogError(err.ToString());
                return Result<RoleResDto>.Failure(err);
            }

            role.Name = request.Dto.Name;
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Role updated with Id={Id}", role.Id);
            return Result<RoleResDto>.Success(new RoleResDto { Id = role.Id, Name = role.Name });
        }
    }
}