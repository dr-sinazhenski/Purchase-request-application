using Application.BusinessLogic.CommentLogic.Dto;
using MediatR;
using Shared;

namespace Application.BusinessLogic.CommentLogic.CreateComment
{
    public record CreateCommentCommand(CrudCommentDto dto) : IRequest<Result<CrudCommentDto>>;
}