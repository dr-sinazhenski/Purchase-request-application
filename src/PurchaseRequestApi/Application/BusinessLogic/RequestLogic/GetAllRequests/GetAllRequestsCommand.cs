using Application.BusinessLogic.RequestLogic.Dto;
using MediatR;
using Shared;

namespace Application.BusinessLogic.RequestLogic.GetAllRequests
{
    public record GetAllRequestsCommand() : IRequest<Result<List<GetAllRequestsDto>>>;
}