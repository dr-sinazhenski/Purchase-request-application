using Application.BusinessLogic.PriceLogic.Dto;
using MediatR;
using Shared;

namespace Application.BusinessLogic.PriceLogic.UpdatePrice
{
    public record UpdatePriceCommand(CrudPriceDto dto) : IRequest<Result<CrudPriceDto>>;
}