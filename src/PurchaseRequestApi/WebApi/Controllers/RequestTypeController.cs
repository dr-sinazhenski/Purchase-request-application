using Application.BusinessLogic.RequestTypeLogic.Dto;
using Application.BusinessLogic.RequestTypeLogic.GetAllRequestTypes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RequestTypeController : ControllerBase
    {
        private readonly ILogger<RequestTypeController> _logger;
        private readonly IMediator _mediator;

        public RequestTypeController(ILogger<RequestTypeController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllRequestTypesCommand());

            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to fetch request types");
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}