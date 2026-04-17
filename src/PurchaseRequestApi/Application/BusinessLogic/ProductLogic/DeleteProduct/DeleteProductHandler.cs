using Application.BusinessLogic.ProductLogic.CreateProduct;
using Application.BusinessLogic.ProductLogic.Dto;
using Infrastructure.Database;
using Infrastructure.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.BusinessLogic.ProductLogic.DeleteProduct
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductRequest, bool>
    {
        private readonly AppDbContext _dbContext;

        public DeleteProductHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(DeleteProductRequest request, CancellationToken cancellationToken)
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == request.id);

            if (product == null)
            {
                return false;
            }

            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
