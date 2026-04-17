using Application;
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
                return BadRequest();
            }

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductReqDto dto)
        { 
            var product = await _mediator.Send(new CreateProductRequest(dto));

            if (product == null)
            {
                return BadRequest();
            }

            return Ok(product);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CreateProductReqDto dto)
        {
            var product = await _mediator.Send(new UpdateProductRequest(dto));

            if (product == null)
            {
                return BadRequest();
            }

            return Ok(product);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteProductRequest(id));

            if (result == false)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
