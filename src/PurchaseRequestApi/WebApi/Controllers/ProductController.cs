using Application;
using Application.BusinessLogic.ProductLogic.CreateProduct;
using Application.BusinessLogic.ProductLogic.DeleteProduct;
using Application.BusinessLogic.ProductLogic.Dto;
using Application.BusinessLogic.ProductLogic.GetProductById;
using Application.BusinessLogic.ProductLogic.UpdateProduct;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;

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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _mediator.Send(new GetProductByIdRequest(id));
            if (product == null)
            {
                _logger.LogError("Product not found");
                return BadRequest();
            }
            
            _logger.LogInformation("Product found");
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductReqDto dto)
        { 
            var product = await _mediator.Send(new CreateProductRequest(dto));

            if (product == null)
            {
                _logger.LogError("Product creation failed");
                return BadRequest();
            }

            _logger.LogInformation("Product creation succeeded");
            return Ok(product);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CreateProductReqDto dto)
        {
            var product = await _mediator.Send(new UpdateProductRequest(dto));

            if (product == null)
            {
                _logger.LogError("Product updating failed");
                return BadRequest();
            }

            _logger.LogInformation("Product update succeeded");
            return Ok(product);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteProductRequest(id));

            if (result == false)
            {
                _logger.LogError("Product deletion failed");
                return BadRequest();
            }

            _logger.LogInformation("Product deletion succeeded");
            return Ok();
        }
    }
}
