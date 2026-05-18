using MediatR;
using Shared;

namespace Application.BusinessLogic.ApproverProfileLogic.DeleteApproverProfile
{
    public record DeleteApproverProfileCommand(Guid Id) : IRequest<Result<bool>>;
}