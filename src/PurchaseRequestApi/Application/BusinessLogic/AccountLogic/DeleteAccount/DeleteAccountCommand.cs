using MediatR;
using Shared;

namespace Application.BusinessLogic.AccountLogic.DeleteAccount
{
    public record DeleteAccountCommand(Guid Id) : IRequest<Result<bool>>;
}