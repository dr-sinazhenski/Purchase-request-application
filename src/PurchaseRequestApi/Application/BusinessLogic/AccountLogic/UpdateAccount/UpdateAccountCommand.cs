using Application.BusinessLogic.AccountLogic.Dto;
using MediatR;
using Shared;

namespace Application.BusinessLogic.AccountLogic.UpdateAccount
{
    public record UpdateAccountCommand(UpdateAccountDto Dto) : IRequest<Result<AccountResDto>>;
}