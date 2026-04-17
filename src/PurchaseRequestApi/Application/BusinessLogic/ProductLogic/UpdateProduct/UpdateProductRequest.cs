using Application.BusinessLogic.ProductLogic.Dto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.BusinessLogic.ProductLogic.UpdateProduct
{
    public record UpdateProductRequest(CreateProductReqDto dto) : IRequest<ProductResDto?>;
}
