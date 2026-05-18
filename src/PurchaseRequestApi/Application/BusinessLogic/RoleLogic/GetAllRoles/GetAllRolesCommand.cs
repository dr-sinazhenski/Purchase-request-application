using Application.BusinessLogic.RoleLogic.Dto;
using MediatR;
using Shared;

namespace Application.BusinessLogic.RoleLogic.GetAllRoles
{
    public record GetAllRolesCommand() : IRequest<Result<List<RoleResDto>>>;
}