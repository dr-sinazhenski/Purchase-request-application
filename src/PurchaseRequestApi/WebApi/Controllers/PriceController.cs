using Application.BusinessLogic.PriceLogic.CreatePrice;
using Application.BusinessLogic.PriceLogic.DeletePrice;
using Application.BusinessLogic.PriceLogic.Dto;
using Application.BusinessLogic.PriceLogic.GetPrice;
using Application.BusinessLogic.PriceLogic.UpdatePrice;
using Application.BusinessLogic.PriceLogic.GetAllPrices;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PriceController : ControllerBase
    {
        private readonly ILogger<PriceController> _logger;
        private readonly IMediator _mediator;

        public PriceController(ILogger<PriceController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllPricesCommand());

            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to fetch prices");
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrudPriceDto dto)
        {
            var result = await _mediator.Send(new CreatePriceCommand(dto));

            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to create price");
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("{productId}/{regionId}")]
        public async Task<IActionResult> GetById(Guid productId, Guid regionId)
        {
            var result = await _mediator.Send(new GetPriceCommand(productId, regionId));

            if (!result.IsSuccess)
            {
                _logger.LogError("Price for ProductId {ProductId} RegionId {RegionId} not found", productId, regionId);
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CrudPriceDto dto)
        {
            var result = await _mediator.Send(new UpdatePriceCommand(dto));

            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to update price");
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("{productId}/{regionId}")]
        public async Task<IActionResult> Delete(Guid productId, Guid regionId)
        {
            var result = await _mediator.Send(new DeletePriceCommand(productId, regionId));

            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to delete price for ProductId {ProductId} RegionId {RegionId}", productId, regionId);
                return BadRequest(result);
            }

            _logger.LogInformation("Price for ProductId {ProductId} RegionId {RegionId} deleted", productId, regionId);
            return Ok(result);
        }
    }
}