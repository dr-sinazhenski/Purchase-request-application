using Application.BusinessLogic.CommentLogic.Dto;
using Infrastructure.Database;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.CommentLogic.UpdateComment
{
    public class UpdateCommentHandler : IRequestHandler<UpdateCommentCommand, Result<CrudCommentDto>>
    {
        private readonly ILogger<UpdateCommentHandler> _logger;
        private readonly AppDbContext _dbContext;

        public UpdateCommentHandler(AppDbContext dbContext, ILogger<UpdateCommentHandler> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<Result<CrudCommentDto>> Handle(UpdateCommentCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating comment {Id}", command.dto.Id);

            var comment = _dbContext.Comments.FirstOrDefault(x => x.Id == command.dto.Id);
            if (comment == null)
            {
                var err = new Error(404, $"Comment with id={command.dto.Id} not found");
                _logger.LogError(err.ToString());
                return Result<CrudCommentDto>.Failure(err);
            }

            comment.Text = command.dto.Text;

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