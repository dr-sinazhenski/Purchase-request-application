using Application.BusinessLogic.RoleLogic.Dto;
using MediatR;
using Shared;

namespace Application.BusinessLogic.RoleLogic.UpdateRole
{
    public record UpdateRoleCommand(Guid Id, CrudRoleDto Dto) : IRequest<Result<RoleResDto>>;
}