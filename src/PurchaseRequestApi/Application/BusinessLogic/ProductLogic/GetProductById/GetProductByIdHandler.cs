using Application.BusinessLogic.ProductLogic.Dto;
using Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.ProductLogic.GetProductById
{
    public class GetProductByIdHandler : IRequestHandler<GetProductByIdRequest, Result<ProductResDto>>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<GetProductByIdHandler> _logger;

        public GetProductByIdHandler(AppDbContext dbContext, ILogger<GetProductByIdHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<ProductResDto>> Handle(GetProductByIdRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting a Product");
            var product = await _dbContext.Products
                .Include(p => p.RequestType)
                .FirstOrDefaultAsync(x => x.Id == request.id);

            /*if (product == null)
            {
                _logger.Error("Product not found");
                return Result<ProductResDto>.Failure(null);
            }*/

            _logger.LogInformation("Product retrived");
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
