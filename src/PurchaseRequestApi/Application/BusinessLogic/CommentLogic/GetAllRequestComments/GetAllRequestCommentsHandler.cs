using Application.BusinessLogic.CommentLogic.Dto;
using Infrastructure.Database;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Application.BusinessLogic.CommentLogic.GetAllRequestComments
{
    public class GetAllRequestCommentsHandler : IRequestHandler<GetAllRequestCommentsCommand, Result<List<CrudCommentDto>>>
    {
        private readonly ILogger<GetAllRequestCommentsHandler> _logger; 
        private readonly AppDbContext _dbContext;

        public GetAllRequestCommentsHandler(AppDbContext dbContext, ILogger<GetAllRequestCommentsHandler> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<Result<List<CrudCommentDto>>> Handle(GetAllRequestCommentsCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching comments for request {Id}", command.Id);

            var query = _dbContext.Comments
                .AsQueryable();

            query = query.Where(r => r.RequestId == command.Id);

            var comments = await query
                .Select(r => new CrudCommentDto
                {
                    Id = r.Id,
                    RequestId = r.RequestId,
                    AccountId = r.AccountId,
                    Text = r.Text,
                    CreationTime = r.CreationTime
                })
                .ToListAsync(cancellationToken);

            return Result<List<CrudCommentDto>>.Success(comments);
        }
    }
}