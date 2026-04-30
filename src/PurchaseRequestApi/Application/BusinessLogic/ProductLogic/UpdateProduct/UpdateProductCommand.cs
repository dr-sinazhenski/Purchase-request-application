using Application.BusinessLogic.ProductLogic.Dto;
using MediatR;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.BusinessLogic.ProductLogic.UpdateProduct
{
    public record UpdateProductCommand(CreateProductDto dto) : IRequest<Result<ProductResDto?>>;
}
