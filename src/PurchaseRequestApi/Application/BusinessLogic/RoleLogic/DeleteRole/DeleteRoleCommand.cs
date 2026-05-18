using MediatR;
using Shared;

namespace Application.BusinessLogic.RoleLogic.DeleteRole
{
    public record DeleteRoleCommand(Guid Id) : IRequest<Result<bool>>;
}