using Application.BusinessLogic.PriceLogic.Dto;
using MediatR;
using Shared;

namespace Application.BusinessLogic.PriceLogic.CreatePrice
{
    public record CreatePriceCommand(CrudPriceDto dto) : IRequest<Result<CrudPriceDto>>;
}