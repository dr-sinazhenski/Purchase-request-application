using Application.BusinessLogic.ProductLogic.CreateProduct;
using Application.BusinessLogic.RequestLogic.Dto;
using Application.BusinessLogic.RequestTypeLogic.Dto;
using Infrastructure.Database;
using Infrastructure.Database.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.BusinessLogic.RequestLogic.CreateRequest
{
    public class CreateRequestHandler : IRequestHandler<CreateRequestCommand, Result<GetRequestDetailsResDto>>
    {
        private readonly ILogger<CreateRequestHandler> _logger;
        private readonly AppDbContext _dbContext;

        public CreateRequestHandler(AppDbContext dbContext, ILogger<CreateRequestHandler> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<Result<GetRequestDetailsResDto>> Handle(CreateRequestCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating new request");

            var type = _dbContext.RequestTypes.FirstOrDefault(x => x.Id == command.dto.requestTypeId);
            if (type == null)
            {

            }

            var request = new Request
            {
                Id = Guid.NewGuid(),
                Title = command.dto.Title,
                Description = command.dto.Description,
                RequestTypeId = type.Id,
                RequestType = type,
                Status = RequestStatus.Submited,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _dbContext.Requests.AddAsync(request);
            await _dbContext.SaveChangesAsync();

            var reqDto = new GetRequestDetailsResDto()
            {
                Id = request.Id,
                Title = request.Title,
                Description = request.Description,
                RequestType = new RequestTypeResDto()
                {
                    Id = request.RequestType.Id,
                    Name = request.RequestType.Name,
                },
                Status = request.Status.ToString(),
                CreatedAt = request.CreatedAt,
                UpdatedAt = DateTime.UtcNow
            };

            return Result<GetRequestDetailsResDto>.Success(reqDto);
        }
    }
}
