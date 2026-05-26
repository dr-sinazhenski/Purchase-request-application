using Application.BusinessLogic.AccountLogic.Dto;
using MediatR;
using Shared;

namespace Application.BusinessLogic.AccountLogic.Login
{
    public record LoginCommand(LoginDto dto) : IRequest<Result<LoginResDto>>;
}