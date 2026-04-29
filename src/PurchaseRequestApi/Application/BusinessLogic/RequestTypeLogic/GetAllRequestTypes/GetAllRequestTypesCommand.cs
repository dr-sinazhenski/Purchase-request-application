using Application.BusinessLogic.RequestTypeLogic.Dto;
using MediatR;
using Shared;

namespace Application.BusinessLogic.RequestTypeLogic.GetAllRequestTypes
{
    public record GetAllRequestTypesCommand() : IRequest<Result<List<RequestTypeResDto>>>;
}