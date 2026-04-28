using Application.BusinessLogic.ProductLogic.DeleteProduct;
using Application.BusinessLogic.ProductLogic.Dto;
using Infrastructure.Database;
using Infrastructure.Database.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.ProductLogic.CreateProduct
{
    public class CreateProductHandler : IRequestHandler<CreateProductRequest, Result<ProductResDto>>
    {
        private readonly ILogger<CreateProductHandler> _logger;
        private readonly AppDbContext _dbContext;

        public CreateProductHandler(AppDbContext dbContext, ILogger<CreateProductHandler> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<Result<ProductResDto>> Handle(CreateProductRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating a Product");

            var product = new Product()
            {
                Id = Guid.NewGuid(),
                Name = request.dto.Name,
                Description = request.dto.Description
            };

            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Product created successfuly");

            var data = new ProductResDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description
            };

            return Result<ProductResDto>.Success(data);
        }
    }
}
