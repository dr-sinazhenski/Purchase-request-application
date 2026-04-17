using Application.BusinessLogic.ProductLogic.Dto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.BusinessLogic.ProductLogic.GetProductById
{
    public record GetProductByIdRequest(Guid id) : IRequest<ProductResDto?>;
}
