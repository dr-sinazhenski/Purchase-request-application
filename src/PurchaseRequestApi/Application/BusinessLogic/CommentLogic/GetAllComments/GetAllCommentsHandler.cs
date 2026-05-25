using Application.BusinessLogic.CommentLogic.Dto;
using Infrastructure.Database;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace Application.BusinessLogic.CommentLogic.GetAllComments
{
    public class GetAllCommentsHandler : IRequestHandler<GetAllCommentsCommand, Result<List<CrudCommentDto>>>
    {
        private readonly ILogger<GetAllCommentsHandler> _logger;
        private readonly AppDbContext _dbContext;

        public GetAllCommentsHandler(AppDbContext dbContext, ILogger<GetAllCommentsHandler> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<Result<List<CrudCommentDto>>> Handle(GetAllCommentsCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching all comments");

            var comments = _dbContext.Comments.Select(c => new CrudCommentDto
            {
                Id = c.Id,
                RequestId = c.RequestId,
                AccountId = c.AccountId,
                Text = c.Text,
                CreationTime = c.CreationTime
            }).ToList();

            return Result<List<CrudCommentDto>>.Success(comments);
        }
    }
}