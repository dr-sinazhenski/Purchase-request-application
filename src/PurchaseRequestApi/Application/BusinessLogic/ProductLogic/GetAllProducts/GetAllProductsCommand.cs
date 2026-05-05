using Application.BusinessLogic.ProductLogic.Dto;
using MediatR;
using Shared;

namespace Application.BusinessLogic.ProductLogic.GetAllProducts
{
    public record GetAllProductsCommand() : IRequest<Result<List<ProductResDto>>>;
}
