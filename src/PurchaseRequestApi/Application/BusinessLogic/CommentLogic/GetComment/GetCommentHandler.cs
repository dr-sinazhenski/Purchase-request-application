using Application.BusinessLogic.CommentLogic.Dto;
using Infrastructure.Database;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.CommentLogic.GetComment
{
    public class GetCommentHandler : IRequestHandler<GetCommentCommand, Result<CrudCommentDto>>
    {
        private readonly ILogger<GetCommentHandler> _logger;
        private readonly AppDbContext _dbContext;

        public GetCommentHandler(AppDbContext dbContext, ILogger<GetCommentHandler> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<Result<CrudCommentDto>> Handle(GetCommentCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching comment {Id}", command.Id);

            var comment = _dbContext.Comments.FirstOrDefault(x => x.Id == command.Id);
            if (comment == null)
            {
                var err = new Error(404, $"Comment with id={command.Id} not found");
                _logger.LogError(err.ToString());
                return Result<CrudCommentDto>.Failure(err);
            }

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