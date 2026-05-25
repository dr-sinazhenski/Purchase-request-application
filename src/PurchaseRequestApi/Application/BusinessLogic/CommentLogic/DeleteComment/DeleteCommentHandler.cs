using Infrastructure.Database;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.CommentLogic.DeleteComment
{
    public class DeleteCommentHandler : IRequestHandler<DeleteCommentCommand, Result<bool>>
    {
        private readonly ILogger<DeleteCommentHandler> _logger;
        private readonly AppDbContext _dbContext;

        public DeleteCommentHandler(AppDbContext dbContext, ILogger<DeleteCommentHandler> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<Result<bool>> Handle(DeleteCommentCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting comment {Id}", command.Id);

            var comment = _dbContext.Comments.FirstOrDefault(x => x.Id == command.Id);
            if (comment == null)
            {
                var err = new Error(404, $"Comment with id={command.Id} not found");
                _logger.LogError(err.ToString());
                return Result<bool>.Failure(err);
            }

            _dbContext.Comments.Remove(comment);
            await _dbContext.SaveChangesAsync();

            return Result<bool>.Success(true);
        }
    }
}