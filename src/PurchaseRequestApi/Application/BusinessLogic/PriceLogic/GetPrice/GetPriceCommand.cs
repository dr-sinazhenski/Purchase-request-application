using Application.BusinessLogic.PriceLogic.Dto;
using MediatR;
using Shared;

namespace Application.BusinessLogic.PriceLogic.GetPrice
{
    public record GetPriceCommand(Guid ProductId, Guid RegionId, string RequiredCurrency) : IRequest<Result<CrudPriceDto>>;
}