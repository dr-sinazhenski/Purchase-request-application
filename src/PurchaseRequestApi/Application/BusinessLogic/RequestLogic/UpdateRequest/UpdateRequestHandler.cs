using Application.BusinessLogic.RequestLogic.Dto;
using Application.BusinessLogic.RequestTypeLogic.Dto;
using Infrastructure.Database;
using Infrastructure.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.BusinessLogic.RequestLogic.UpdateRequest
{
    public class UpdateRequestHandler : IRequestHandler<UpdateRequestCommand, Result<GetRequestDetailsResDto>>
    {
        private readonly ILogger<UpdateRequestHandler> _logger;
        private readonly AppDbContext _dbContext;

        public UpdateRequestHandler(AppDbContext dbContext, ILogger<UpdateRequestHandler> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<Result<GetRequestDetailsResDto>> Handle(UpdateRequestCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating request");

            var request = await _dbContext.Requests.FirstOrDefaultAsync(x => x.Id == command.dto.Id);

            if (request == null)
            {
                _logger.LogInformation("Reqest not found");
            }

            var type = await _dbContext.RequestTypes.FirstOrDefaultAsync(x => x.Id == command.dto.RequestTypeId);
            if (type == null)
            {
                _logger.LogInformation("Reqest type not found");
            }

            request.Title = command.dto.Title;
            request.Description = command.dto.Description;
            request.RequestTypeId = type.Id;
            request.RequestType = type;
            request.UpdatedAt = DateTime.UtcNow;

            if (request.Status is RequestStatus.Rejected)
            {
                request.Status = RequestStatus.Resubmited;
            }

            await _dbContext.Requests.AddAsync(request);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Reqest updated");

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
