using Application.BusinessLogic.RoleLogic.Dto;
using MediatR;
using Shared;

namespace Application.BusinessLogic.RoleLogic.CreateRole
{
    public record CreateRoleCommand(CrudRoleDto Dto) : IRequest<Result<RoleResDto>>;
}