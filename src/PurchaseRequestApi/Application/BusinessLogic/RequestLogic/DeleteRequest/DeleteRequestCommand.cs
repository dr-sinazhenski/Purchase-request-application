using Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.RequestLogic.DeleteRequest
{
    public class DeleteRequestHandler : IRequestHandler<DeleteRequestCommand, Result>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<DeleteRequestHandler> _logger;

        public DeleteRequestHandler(AppDbContext dbContext, ILogger<DeleteRequestHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result> Handle(DeleteRequestCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting request {Id}", request.Id);

            var r = await _dbContext.Requests
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

            if (r == null)
            {
                _logger.LogWarning("Request {Id} not found for deletion", request.Id);
                return null;
            }

            _dbContext.Requests.Remove(r);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Request {Id} deleted", request.Id);
            return Result.Success();
        }
    }
}