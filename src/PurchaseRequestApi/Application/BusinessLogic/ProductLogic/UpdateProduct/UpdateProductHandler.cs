using Application.BusinessLogic.ProductLogic.Dto;
using Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.ProductLogic.UpdateProduct
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductRequest, Result<ProductResDto?>>
    {
        private readonly ILogger<UpdateProductHandler> _logger;
        private readonly AppDbContext _dbContext;

        public UpdateProductHandler(AppDbContext dbContext, ILogger<UpdateProductHandler> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<Result<ProductResDto>> Handle(UpdateProductRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating a Product");
            var product = await _dbContext.Products
                .Include(p => p.RequestType)
                .FirstOrDefaultAsync(x => x.Id == request.dto.Id);

            if (product == null)
            {
                return null;
            }

            var requestTypes = await _dbContext.RequestTypes
                .Where(rt => request.dto.RequestTypeIds.Contains(rt.Id))
                .ToListAsync(cancellationToken);

            product.Name = request.dto.Name;
            product.Description = request.dto.Description;
            product.RequestType = requestTypes;

            _dbContext.Update(product);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Product updated");

            var data = new ProductResDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                RequestTypeIds = product.RequestType.Select(rt => rt.Id).ToList()
            };

            return Result<ProductResDto>.Success(data);
        }
    }
}
