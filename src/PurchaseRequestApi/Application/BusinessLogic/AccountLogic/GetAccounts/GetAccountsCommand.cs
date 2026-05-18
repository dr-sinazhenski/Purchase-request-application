using Application.BusinessLogic.AccountLogic.Dto;
using MediatR;
using Shared;

namespace Application.BusinessLogic.AccountLogic.GetAllAccounts
{
    public record GetAllAccountsCommand() : IRequest<Result<List<AccountResDto>>>;
}