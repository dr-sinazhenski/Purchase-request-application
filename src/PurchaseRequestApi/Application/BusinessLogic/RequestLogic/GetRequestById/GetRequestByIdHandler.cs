using Application.BusinessLogic.RequestLogic.Dto;
using Application.BusinessLogic.RequestTypeLogic.Dto;
using Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.RequestLogic.GetRequestById
{
    public class GetRequestByIdHandler : IRequestHandler<GetRequestByIdCommand, Result<GetRequestDetailsResDto>>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<GetRequestByIdHandler> _logger;

        public GetRequestByIdHandler(AppDbContext dbContext, ILogger<GetRequestByIdHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<GetRequestDetailsResDto>> Handle(GetRequestByIdCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching request {Id}", request.Id);

            var r = await _dbContext.Requests
                .AsNoTracking()
                .Include(r => r.RequestType)
                .Include(r => r.RequesterProducts)
                    .ThenInclude(rp => rp.Product)
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

            if (r == null)
            {
                _logger.LogWarning("Request {Id} not found", request.Id);
                return null;
            }

            var data = new GetRequestDetailsResDto
            {
                Id = r.Id,
                Title = r.Title,
                Description = r.Description,
                Status = r.Status.ToString(),
                CreatedAt = r.CreatedAt,
                EditedAt = r.EditedAt,
                RequestType = new RequestTypeResDto
                {
                    Id = r.RequestType.Id,
                    Name = r.RequestType.Name
                },
            };

            return Result<GetRequestDetailsResDto>.Success(data);
        }
    }
}