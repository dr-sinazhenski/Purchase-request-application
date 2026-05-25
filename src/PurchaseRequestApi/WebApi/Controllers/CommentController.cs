using Application.BusinessLogic.CommentLogic.CreateComment;
using Application.BusinessLogic.CommentLogic.DeleteComment;
using Application.BusinessLogic.CommentLogic.Dto;
using Application.BusinessLogic.CommentLogic.GetAllComments;
using Application.BusinessLogic.CommentLogic.GetAllRequestComments;
using Application.BusinessLogic.CommentLogic.GetComment;
using Application.BusinessLogic.CommentLogic.UpdateComment;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ILogger<CommentController> _logger;
        private readonly IMediator _mediator;

        public CommentController(ILogger<CommentController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrudCommentDto dto)
        {
            var result = await _mediator.Send(new CreateCommentCommand(dto));

            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to create comment for RequestId {RequestId}", dto.RequestId);
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllCommentsCommand());

            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to fetch comments");
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetCommentCommand(id));

            if (!result.IsSuccess)
            {
                _logger.LogError("Comment {Id} not found", id);
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("request/{id}")]
        public async Task<IActionResult> GetByRequestId(Guid id)
        {
            var result = await _mediator.Send(new GetAllRequestCommentsCommand(id));

            if (!result.IsSuccess)
            {
                _logger.LogError("Comments for request with {Id} not found", id);
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CrudCommentDto dto)
        {
            var result = await _mediator.Send(new UpdateCommentCommand(dto));

            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to update comment {Id}", dto.Id);
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteCommentCommand(id));

            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to delete comment {Id}", id);
                return BadRequest(result);
            }

            _logger.LogInformation("Comment {Id} deleted", id);
            return Ok(result);
        }
    }
}