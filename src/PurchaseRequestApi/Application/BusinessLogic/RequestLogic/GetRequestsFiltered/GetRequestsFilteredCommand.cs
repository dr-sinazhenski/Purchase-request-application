using Application.BusinessLogic.RequestLogic.Dto;
using MediatR;
using Shared;

namespace Application.BusinessLogic.RequestLogic.GetRequestsFiltered
{
    public record GetRequestsFilteredCommand(
        Guid? RequestTypeId,
        string? Status,
        Guid? RegionId,
        string RequiredCurrency
    ) : IRequest<Result<List<GetRequestsFilteredDto>>>;
}