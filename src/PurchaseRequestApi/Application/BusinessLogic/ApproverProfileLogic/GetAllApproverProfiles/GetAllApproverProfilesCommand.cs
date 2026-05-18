using Application.BusinessLogic.ApproverProfileLogic.Dto;
using MediatR;
using Shared;

namespace Application.BusinessLogic.ApproverProfileLogic.GetAllApproverProfiles
{
    public record GetAllApproverProfilesCommand() : IRequest<Result<List<ApproverProfileResDto>>>;
}