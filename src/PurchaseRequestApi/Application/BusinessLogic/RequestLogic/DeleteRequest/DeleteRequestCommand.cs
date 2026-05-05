using MediatR;
using Shared;

namespace Application.BusinessLogic.RequestLogic.DeleteRequest
{
    public record DeleteRequestCommand(Guid Id) : IRequest<Result>;
}