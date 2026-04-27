using Application.BusinessLogic.ProductLogic.Dto;
using Infrastructure.Database;
using Infrastructure.Database.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using AutoWrapper;
using Serilog;

namespace Application.BusinessLogic.ProductLogic.CreateProduct
{
    public class CreateProductHandler : IRequestHandler<CreateProductRequest, ProductResDto?>
    {
        private readonly ILogger _logger;
        private readonly AppDbContext _dbContext;

        public CreateProductHandler(AppDbContext dbContext, ILogger logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<ProductResDto> Handle(CreateProductRequest request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.Information("==================Creating a Product================");
                var product = new Product()
                {
                    Id = Guid.NewGuid(),
                    Name = request.dto.Name,
                    Description = request.dto.Description
                };

                await _dbContext.Products.AddAsync(product);
                await _dbContext.SaveChangesAsync();

                return new ProductResDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description
                };
            }
            catch(Exception exception)
            {
                _logger.Information("==================EXCEPTION!================");
                throw;
            }
        }
    }
}
