using Application.BusinessLogic.RoleLogic.Dto;
using Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.RoleLogic.GetAllRoles
{
    public class GetAllRolesHandler : IRequestHandler<GetAllRolesCommand, Result<List<RoleResDto>>>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<GetAllRolesHandler> _logger;

        public GetAllRolesHandler(AppDbContext dbContext, ILogger<GetAllRolesHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<List<RoleResDto>>> Handle(GetAllRolesCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching all roles");

            var roles = await _dbContext.Roles
                .AsNoTracking()
                .Select(r => new RoleResDto { Id = r.Id, Name = r.Name })
                .ToListAsync(cancellationToken);

            return Result<List<RoleResDto>>.Success(roles);
        }
    }
}