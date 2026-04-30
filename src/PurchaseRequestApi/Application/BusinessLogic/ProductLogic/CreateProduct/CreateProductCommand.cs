using Application.BusinessLogic.ProductLogic.Dto;
using MediatR;
using Shared;

namespace Application.BusinessLogic.ProductLogic.CreateProduct
{
    public record CreateProductCommand(CreateProductDto dto) : IRequest<Result<ProductResDto>>;
}
