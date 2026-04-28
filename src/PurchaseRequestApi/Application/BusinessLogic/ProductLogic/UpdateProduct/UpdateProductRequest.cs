using Application.BusinessLogic.ProductLogic.Dto;
using MediatR;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.BusinessLogic.ProductLogic.UpdateProduct
{
    public record UpdateProductRequest(CreateProductReqDto dto) : IRequest<Result<ProductResDto?>>;
}
