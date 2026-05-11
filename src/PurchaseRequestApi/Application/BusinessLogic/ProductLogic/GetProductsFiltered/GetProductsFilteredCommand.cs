using Application.BusinessLogic.ProductLogic.Dto;
using MediatR;
using Shared;

namespace Application.BusinessLogic.ProductLogic.GetProductsFiltered
{
    public record GetProductsFilteredCommand(
        Guid? RegionId,
        Guid? RequestTypeId
    ) : IRequest<Result<List<GetFilteredProductDto>>>;
}