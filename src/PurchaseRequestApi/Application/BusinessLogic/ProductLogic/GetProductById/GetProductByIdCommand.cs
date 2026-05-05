using Application.BusinessLogic.ProductLogic.Dto;
using MediatR;
using Shared;

namespace Application.BusinessLogic.ProductLogic.GetProductById
{
    public record GetProductByIdCommand(Guid Id) : IRequest<Result<ProductResDto>>;
}
