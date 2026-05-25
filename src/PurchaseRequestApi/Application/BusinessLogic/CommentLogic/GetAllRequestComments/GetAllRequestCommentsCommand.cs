using Application.BusinessLogic.CommentLogic.Dto;
using MediatR;
using Shared;

namespace Application.BusinessLogic.CommentLogic.GetAllRequestComments
{
    public record GetAllRequestCommentsCommand(Guid Id) : IRequest<Result<List<CrudCommentDto>>>;
}