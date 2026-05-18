using Application.BusinessLogic.ApproverProfileLogic.Dto;
using MediatR;
using Shared;

namespace Application.BusinessLogic.ApproverProfileLogic.CreateApproverProfile
{
    public record CreateApproverProfileCommand(CrudApproverProfileDto Dto) : IRequest<Result<ApproverProfileResDto>>;
}