using Application.BusinessLogic.CommentLogic.Dto;
using Infrastructure.Database;
using Infrastructure.Database.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.CommentLogic.CreateComment
{
    public class CreateCommentHandler : IRequestHandler<CreateCommentCommand, Result<CrudCommentDto>>
    {
        private readonly ILogger<CreateCommentHandler> _logger;
        private readonly AppDbContext _dbContext;

        public CreateCommentHandler(AppDbContext dbContext, ILogger<CreateCommentHandler> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<Result<CrudCommentDto>> Handle(CreateCommentCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating comment for RequestId {RequestId}", command.dto.RequestId);

            var request = _dbContext.Requests.FirstOrDefault(x => x.Id == command.dto.RequestId);
            if (request == null)
            {
                var err = new Error(404, $"Request with id={command.dto.RequestId} not found");
                _logger.LogError(err.ToString());
                return Result<CrudCommentDto>.Failure(err);
            }

            if (command.dto.AccountId.HasValue)
            {
                var account = _dbContext.Accounts.FirstOrDefault(x => x.Id == command.dto.AccountId);
                if (account == null)
                {
                    var err = new Error(404, $"Account with id={command.dto.AccountId} not found");
                    _logger.LogError(err.ToString());
                    return Result<CrudCommentDto>.Failure(err);
                }
            }

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                RequestId = command.dto.RequestId,
                AccountId = command.dto.AccountId,
                Text = command.dto.Text,
                CreationTime = DateTime.UtcNow,
                Request = request
            };

            await _dbContext.Comments.AddAsync(comment);
            await _dbContext.SaveChangesAsync();

            return Result<CrudCommentDto>.Success(new CrudCommentDto
            {
                Id = comment.Id,
                RequestId = comment.RequestId,
                AccountId = comment.AccountId,
                Text = comment.Text,
                CreationTime = comment.CreationTime
            });
        }
    }
}