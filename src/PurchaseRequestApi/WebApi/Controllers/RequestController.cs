using Application.BusinessLogic.RequestLogic.CreateRequest;
using Application.BusinessLogic.RequestLogic.Dto;
using Application.BusinessLogic.RequestLogic.UpdateRequest;
using Application.BusinessLogic.RequestTypeLogic.Dto;
using Application.BusinessLogic.RequestTypeLogic.GetAllRequestTypes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
    }
}