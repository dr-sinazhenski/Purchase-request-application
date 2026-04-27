using Application.BusinessLogic.ProductLogic.Dto;
using Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Serilog;

namespace Application.BusinessLogic.ProductLogic.UpdateProduct
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductRequest, ProductResDto?>
    {
        private readonly ILogger _logger;
        private readonly AppDbContext _dbContext;

        public UpdateProductHandler(AppDbContext dbContext, ILogger logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<ProductResDto> Handle(UpdateProductRequest request, CancellationToken cancellationToken)
        {
            _logger.Information("==================Updating a Product================");
            var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == request.dto.Id);

            if (product == null)
            {
                return null;
            }

            product.Name = request.dto.Name;
            product.Description = request.dto.Description;

            _dbContext.Update(product);
            await _dbContext.SaveChangesAsync();

            return new ProductResDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description
            };
        }
    }
}
