using MediatR;
using Shared;

namespace Application.BusinessLogic.CommentLogic.DeleteComment
{
    public record DeleteCommentCommand(Guid Id) : IRequest<Result<bool>>;
}