using Application.BusinessLogic.CommentLogic.Dto;
using MediatR;
using Shared;

namespace Application.BusinessLogic.CommentLogic.UpdateComment
{
    public record UpdateCommentCommand(CrudCommentDto dto) : IRequest<Result<CrudCommentDto>>;
}