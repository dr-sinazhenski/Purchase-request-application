using Application.BusinessLogic.RequestLogic.Dto;
using Application.BusinessLogic.RequestLogic.UpdateRequest;
using Infrastructure.Database;
using Infrastructure.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;
namespace Application.BusinessLogic.RequestLogic.RejectRequest
{
    public class RejectRequestHandler : IRequestHandler<RejectRequestCommand, Result>
    {
        private readonly ILogger<RejectRequestHandler> _logger;
        private readonly AppDbContext _dbContext;

        public RejectRequestHandler(AppDbContext dbContext, ILogger<RejectRequestHandler> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<Result> Handle(RejectRequestCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Rejecting request");

            var request = await _dbContext.Requests.FirstOrDefaultAsync(x => x.Id == command.dto.Id);
            if (request == null)
            {
                _logger.LogInformation("Reqest not found");
            }
            if (request.Status != RequestStatus.Submited && request.Status != RequestStatus.Resubmited)
            {
                _logger.LogInformation("Wrong request status");
            }

            var comment = new Comment
            {
                Text = command.dto.Reason,
                CreationTime = DateTime.UtcNow,
                RequestId = request.Id,
                Request = request
            };
            await _dbContext.Comments.AddAsync(comment);

            request.Status = command.dto.IsFinal ? RequestStatus.FinalReject : RequestStatus.Rejected;
            request.RejectionCommentId = comment.Id;
            request.RejectionComment = comment;

            _dbContext.Requests.Update(request);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Request rejected");

            return Result.Success();
        }
    }
}