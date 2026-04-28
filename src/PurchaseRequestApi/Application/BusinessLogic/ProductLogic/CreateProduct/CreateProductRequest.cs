using Application.BusinessLogic.ProductLogic.Dto;
using MediatR;
using Shared;

namespace Application.BusinessLogic.ProductLogic.CreateProduct
{
    public record CreateProductRequest(CreateProductReqDto dto) : IRequest<Result<ProductResDto>>;
}
