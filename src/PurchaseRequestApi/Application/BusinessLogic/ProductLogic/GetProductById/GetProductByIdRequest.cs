using Application.BusinessLogic.ProductLogic.Dto;
using MediatR;
using Shared;

namespace Application.BusinessLogic.ProductLogic.GetProductById
{
    public record GetProductByIdRequest(Guid id) : IRequest<Result<ProductResDto>>;
}
