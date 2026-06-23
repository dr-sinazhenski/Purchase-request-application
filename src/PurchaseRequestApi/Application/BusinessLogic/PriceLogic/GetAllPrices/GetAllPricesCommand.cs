using Application.BusinessLogic.PriceLogic.Dto;
using MediatR;
using Shared;

namespace Application.BusinessLogic.PriceLogic.GetAllPrices
{
    public record GetAllPricesCommand(string RequiredCurrency) : IRequest<Result<List<CrudPriceDto>>>;
}