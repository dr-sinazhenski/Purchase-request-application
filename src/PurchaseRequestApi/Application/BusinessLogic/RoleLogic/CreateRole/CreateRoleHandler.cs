using Application.BusinessLogic.RoleLogic.Dto;
using Infrastructure.Database;
using Infrastructure.Database.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.RoleLogic.CreateRole
{
    public class CreateRoleHandler : IRequestHandler<CreateRoleCommand, Result<RoleResDto>>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<CreateRoleHandler> _logger;

        public CreateRoleHandler(AppDbContext dbContext, ILogger<CreateRoleHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<RoleResDto>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating role with Name={Name}", request.Dto.Name);

            var existing = _dbContext.Roles.FirstOrDefault(r => r.Name == request.Dto.Name);
            if (existing is not null)
            {
                var err = new Error(400, $"Role with name={request.Dto.Name} already exists");
                _logger.LogError(err.ToString());
                return Result<RoleResDto>.Failure(err);
            }

            var role = new Role { Name = request.Dto.Name, Account = new List<Account>() };

            await _dbContext.Roles.AddAsync(role);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Role created with Id={Id}", role.Id);
            return Result<RoleResDto>.Success(new RoleResDto { Id = role.Id, Name = role.Name });
        }
    }
}