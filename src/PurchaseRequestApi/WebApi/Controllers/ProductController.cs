using Application;
using Application.BusinessLogic.ProductLogic.CreateProduct;
using Application.BusinessLogic.ProductLogic.DeleteProduct;
using Application.BusinessLogic.ProductLogic.Dto;
using Application.BusinessLogic.ProductLogic.GetAllProducts;
using Application.BusinessLogic.ProductLogic.GetProductById;
using Application.BusinessLogic.ProductLogic.UpdateProduct;
using Application.BusinessLogic.RegionLogic.GetAllRegions;
using Application.BusinessLogic.ProductLogic.GetProductsFiltered;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IMediator _mediator;

        public ProductController(ILogger<ProductController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllProductsCommand());

            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to fetch products");
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetProductByIdCommand(id));

            if (!result.IsSuccess)
            {
                _logger.LogError("Product not found");
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        { 
            var result = await _mediator.Send(new CreateProductCommand(dto));

            if (!result.IsSuccess)
            {
                _logger.LogError("Product creation failed");
                return BadRequest(result);
            }

            _logger.LogInformation("Product creation succeeded");
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CreateProductDto dto)
        {
            var result = await _mediator.Send(new UpdateProductCommand(dto));

            if (!result.IsSuccess)
            {
                _logger.LogError("Product updating failed");
                return BadRequest(result);
            }

            _logger.LogInformation("Product update succeeded");
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteProductCommand(id));

            if (!result.IsSuccess)
            {
                _logger.LogError("Product deletion failed");
                return BadRequest(result);
            }   

            _logger.LogInformation("Product deletion succeeded");
            return Ok(result);
        }
        [HttpGet("filtered")]
        public async Task<IActionResult> GetFiltered(
            [FromQuery] Guid? regionId,
            [FromQuery] Guid? requestTypeId)
        {
            var result = await _mediator.Send(new GetProductsFilteredCommand(regionId, requestTypeId));

            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to fetch filtered products");
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
