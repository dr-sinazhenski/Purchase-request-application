using Application.BusinessLogic.RequestLogic.CreateRequest;
using Application.BusinessLogic.RequestLogic.Dto;
using Application.BusinessLogic.RequestLogic.GetAllRequests;
using Application.BusinessLogic.RequestLogic.UpdateRequest;
using Application.BusinessLogic.RequestLogic.GetRequestById;
using Application.BusinessLogic.RequestLogic.DeleteRequest;
using Application.BusinessLogic.RequestTypeLogic.Dto;
using Application.BusinessLogic.RequestTypeLogic.GetAllRequestTypes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.BusinessLogic.RequestLogic.ApproveRequest;
using Application.BusinessLogic.RequestLogic.RejectRequest;
using Application.BusinessLogic.RequestLogic.GetRequestsFiltered;
using Microsoft.AspNetCore.Authorization;


namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RequestController : ControllerBase
    {
        private readonly ILogger<RequestTypeController> _logger;
        private readonly IMediator _mediator;

        public RequestController(ILogger<RequestTypeController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRequestDto dto)
        {
            var result = await _mediator.Send(new CreateRequestCommand(dto));

            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to create request");
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllRequestsCommand());

            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to fetch requests");
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetRequestByIdCommand(id));

            if (!result.IsSuccess)
            {
                _logger.LogError("Request {Id} not found", id);
                return NotFound(result);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CreateRequestDto dto)
        {
            var result = await _mediator.Send(new UpdateRequestCommand(dto));

            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to update request");
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteRequestCommand(id));

            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to delete request {Id}", id);
                return BadRequest(result);
            }

            _logger.LogInformation("Request {Id} deleted", id);
            return Ok(result);
        }

        [Authorize(Roles = "Approver")]
        [HttpPut("/Approve/{id}")]
        public async Task<IActionResult> Approve(Guid id)
        {
            var result = await _mediator.Send(new ApproveRequestCommand(id));

            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to approve request {Id}", id);
                return BadRequest(result);
            }

            _logger.LogInformation("Request {Id} approved", id);
            return Ok(result);
        }

        [Authorize(Roles = "Approver")]
        [HttpPut("/Reject/")]
        public async Task<IActionResult> Reject([FromBody] RejectRequestDto dto)
        {
            var result = await _mediator.Send(new RejectRequestCommand(dto));

            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to reject request {Id}", dto.Id);
                return BadRequest(result);
            }

            _logger.LogInformation("Request {Id} reject", dto.Id);
            return Ok(result);
        }
        
        [Authorize]
        [HttpGet("filtered")]
        public async Task<IActionResult> GetFiltered(
            [FromQuery] Guid? requestTypeId,
            [FromQuery] string? status,
            [FromQuery] Guid? regionId)
        {
            var result = await _mediator.Send(new GetRequestsFilteredCommand(requestTypeId, status, regionId));

            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to fetch filtered requests");
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}