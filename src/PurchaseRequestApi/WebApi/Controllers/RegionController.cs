using Application.BusinessLogic.RegionLogic.CreateRegion;
using Application.BusinessLogic.RegionLogic.DeleteRegion;
using Application.BusinessLogic.RegionLogic.Dto;
using Application.BusinessLogic.RegionLogic.GetRegion;
using Application.BusinessLogic.RegionLogic.UpdateRegion;
using Application.BusinessLogic.RegionLogic.GetAllRegions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegionController : ControllerBase
    {
        private readonly ILogger<RegionController> _logger;
        private readonly IMediator _mediator;

        public RegionController(ILogger<RegionController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllRegionsCommand());

            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to fetch regions");
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrudRegionDto dto)
        {
            var result = await _mediator.Send(new CreateRegionCommand(dto));

            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to create region");
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetRegionCommand(id));

            if (!result.IsSuccess)
            {
                _logger.LogError("Region {Id} not found", id);
                return NotFound(result);
            }

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CrudRegionDto dto)
        {
            var result = await _mediator.Send(new UpdateRegionCommand(dto));

            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to update region {Id}", dto.Id);
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteRegionCommand(id));

            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to delete region {Id}", id);
                return BadRequest(result);
            }

            _logger.LogInformation("Region {Id} deleted", id);
            return Ok(result);
        }
    }
}