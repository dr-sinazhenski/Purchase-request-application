using Application.BusinessLogic.CommentLogic.Dto;
using MediatR;
using Shared;

namespace Application.BusinessLogic.CommentLogic.GetAllComments
{
    public record GetAllCommentsCommand() : IRequest<Result<List<CrudCommentDto>>>;
}