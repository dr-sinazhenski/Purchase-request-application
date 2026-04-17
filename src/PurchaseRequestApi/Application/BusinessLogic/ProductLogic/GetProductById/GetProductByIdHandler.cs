using Application.BusinessLogic.ProductLogic.Dto;
using Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.BusinessLogic.ProductLogic.GetProductById
{
    public class GetProductByIdHandler : IRequestHandler<GetProductByIdRequest, ProductResDto?>
    {
        private readonly AppDbContext _dbContext;

        public GetProductByIdHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ProductResDto> Handle(GetProductByIdRequest request, CancellationToken cancellationToken)
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == request.id);

            if (product == null)
            {
                return null;
            }

            return new ProductResDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description
            };
        }
    }
}
