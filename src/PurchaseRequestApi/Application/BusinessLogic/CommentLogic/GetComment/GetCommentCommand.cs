using Application.BusinessLogic.CommentLogic.Dto;
using MediatR;
using Shared;

namespace Application.BusinessLogic.CommentLogic.GetComment
{
    public record GetCommentCommand(Guid Id) : IRequest<Result<CrudCommentDto>>;
}