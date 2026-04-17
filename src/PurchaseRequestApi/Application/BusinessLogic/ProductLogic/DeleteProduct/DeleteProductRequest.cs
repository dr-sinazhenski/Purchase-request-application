using Application.BusinessLogic.ProductLogic.Dto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.BusinessLogic.ProductLogic.DeleteProduct
{
    public record DeleteProductRequest(Guid id) : IRequest<bool>;
}
