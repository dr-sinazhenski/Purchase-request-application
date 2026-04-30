using Application.BusinessLogic.ProductLogic.Dto;
using Application.BusinessLogic.RequestLogic.CreateRequest;
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

namespace Application.BusinessLogic.RequestLogic.ApproveRequest
{
    public class ApproveRequestHandler : IRequestHandler<ApproveRequestCommand, Result>
    {
        private readonly ILogger<ApproveRequestHandler> _logger;
        private readonly AppDbContext _dbContext;

        public ApproveRequestHandler(AppDbContext dbContext, ILogger<ApproveRequestHandler> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<Result> Handle(ApproveRequestCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Approving request");
            var request = await _dbContext.Requests.FirstOrDefaultAsync(x => x.Id == command.id);
            if (request == null)
            {
                _logger.LogInformation("Request not found");
            }
            if (request.Status != RequestStatus.Submited && request.Status != RequestStatus.Resubmited)
            {
                _logger.LogInformation("Wrong request status");
            }

            request.Status = RequestStatus.Approved;

            _dbContext.Requests.Update(request);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Request approved");

            return Result.Success();
        }
    }
}
