using Application.BusinessLogic.ProductLogic.CreateProduct;
using Application.BusinessLogic.ProductLogic.DeleteProduct;
using Application.BusinessLogic.ProductLogic.Dto;
using Application.BusinessLogic.ProductLogic.GetProductById;
using Application.BusinessLogic.ProductLogic.UpdateProduct;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : PurchaseController
    {
        public ProductController(ILogger logger, IMediator mediator) : base(logger, mediator)
        {
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetProductByIdRequest(id));

            if (!result.IsSuccess)
            {
                _logger.LogError("Product not found");
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductReqDto dto)
        { 
            var result = await _mediator.Send(new CreateProductRequest(dto));

            if (!result.IsSuccess)
            {
                _logger.LogError("Product creation failed");
                return BadRequest(result);
            }

            _logger.LogInformation("Product creation succeeded");
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CreateProductReqDto dto)
        {
            var result = await _mediator.Send(new UpdateProductRequest(dto));

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
            var result = await _mediator.Send(new DeleteProductRequest(id));

            if (!result.IsSuccess)
            {
                _logger.LogError("Product deletion failed");
                return BadRequest(result);
            }   

            _logger.LogInformation("Product deletion succeeded");
            return Ok(result);
        }
    }
}
