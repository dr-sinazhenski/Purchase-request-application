using Application.BusinessLogic.ApproverProfileLogic.Dto;
using MediatR;
using Shared;

namespace Application.BusinessLogic.ApproverProfileLogic.UpdateApproverProfile
{
    public record UpdateApproverProfileCommand(Guid Id, CrudApproverProfileDto Dto) : IRequest<Result<ApproverProfileResDto>>;
}