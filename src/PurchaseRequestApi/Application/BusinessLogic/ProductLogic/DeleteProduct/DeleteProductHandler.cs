using Application.BusinessLogic.RequestLogic.Dto;
using Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.ProductLogic.DeleteProduct
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, Result>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<DeleteProductHandler> _logger;

        public DeleteProductHandler(AppDbContext dbContext, ILogger<DeleteProductHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting a Product");
            var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == request.Id);

            if (product == null)
            {
                var err = new Error(404, $"Product with id= {request.Id} not found");
                _logger.LogError(err.ToString());
                return Result<GetRequestDetailsResDto>.Failure(err);
            }

            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Product deleted");

            return Result.Success();
        }
    }
}
