using Application.BusinessLogic.RequestLogic.Dto;
using MediatR;
using Shared;

namespace Application.BusinessLogic.RequestLogic.GetRequestById
{
    public record GetRequestByIdCommand(Guid Id) : IRequest<Result<GetRequestDetailsResDto>>;
}