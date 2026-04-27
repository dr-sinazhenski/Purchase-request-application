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
        private readonly Serilog.ILogger _logger;
        private readonly IMediator _mediator;

        public ProductController(Serilog.ILogger logger, IMediator mediator)
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
                _logger.Error("Product not found");
                return BadRequest();
            }
            
            _logger.Information("Product found");
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductReqDto dto)
        { 
            var product = await _mediator.Send(new CreateProductRequest(dto));

            if (product == null)
            {
                _logger.Error("Product creation failed");
                return BadRequest();
            }

            _logger.Information("Product creation succeeded");
            return Ok(product);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CreateProductReqDto dto)
        {
            var product = await _mediator.Send(new UpdateProductRequest(dto));

            if (product == null)
            {
                _logger.Error("Product updating failed");
                return BadRequest();
            }

            _logger.Information("Product update succeeded");
            return Ok(product);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteProductRequest(id));

            if (result == false)
            {
                _logger.Error("Product deletion failed");
                return BadRequest();
            }   

            _logger.Information("Product deletion succeeded");
            return Ok();
        }
    }
}
