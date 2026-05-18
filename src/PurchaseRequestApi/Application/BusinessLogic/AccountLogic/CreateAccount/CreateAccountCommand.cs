using Application.BusinessLogic.AccountLogic.Dto;
using MediatR;
using Shared;

namespace Application.BusinessLogic.AccountLogic.CreateAccount
{
    public record CreateAccountCommand(CreateAccountDto Dto) : IRequest<Result<AccountResDto>>;
}