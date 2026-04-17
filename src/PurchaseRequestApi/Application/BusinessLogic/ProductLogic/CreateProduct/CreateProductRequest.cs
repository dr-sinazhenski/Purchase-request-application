using Application.BusinessLogic.ProductLogic.Dto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.BusinessLogic.ProductLogic.CreateProduct
{
    public record CreateProductRequest(CreateProductReqDto dto) : IRequest<ProductResDto>;
}
