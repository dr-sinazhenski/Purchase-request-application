using Application.BusinessLogic.ProductLogic.Dto;
using Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Shared;

namespace Application.BusinessLogic.ProductLogic.UpdateProduct
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductRequest, Result<ProductResDto?>>
    {
        private readonly ILogger _logger;
        private readonly AppDbContext _dbContext;

        public UpdateProductHandler(AppDbContext dbContext, ILogger logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<Result<ProductResDto>> Handle(UpdateProductRequest request, CancellationToken cancellationToken)
        {
            _logger.Information("Updating a Product");
            var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == request.dto.Id);

            if (product == null)
            {
                return null;
            }

            product.Name = request.dto.Name;
            product.Description = request.dto.Description;

            _dbContext.Update(product);
            await _dbContext.SaveChangesAsync();
            _logger.Information("Product updated");

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
