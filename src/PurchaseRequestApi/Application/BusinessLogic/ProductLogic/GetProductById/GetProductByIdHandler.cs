using Application.BusinessLogic.ProductLogic.Dto;
using Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Serilog;

namespace Application.BusinessLogic.ProductLogic.GetProductById
{
    public class GetProductByIdHandler : IRequestHandler<GetProductByIdRequest, ProductResDto?>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger _logger;

        public GetProductByIdHandler(AppDbContext dbContext, ILogger logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<ProductResDto> Handle(GetProductByIdRequest request, CancellationToken cancellationToken)
        {
            _logger.Information("==================Getting a Product================");
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
