using Application.BusinessLogic.ProductLogic.Dto;
using Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Shared;

namespace Application.BusinessLogic.ProductLogic.GetProductById
{
    public class GetProductByIdHandler : IRequestHandler<GetProductByIdRequest, Result<ProductResDto>>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger _logger;

        public GetProductByIdHandler(AppDbContext dbContext, ILogger logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<ProductResDto>> Handle(GetProductByIdRequest request, CancellationToken cancellationToken)
        {
            _logger.Information("Getting a Product");
            var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == request.id);

            /*if (product == null)
            {
                _logger.Error("Product not found");
                return Result<ProductResDto>.Failure(null);
            }*/

            _logger.Information("Product retrived");
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
