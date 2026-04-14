using Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly PlaceholderHandler _placeholderHandler;

        public ProductController(ILogger<ProductController> logger, PlaceholderHandler placeholderHandler)
        {
            _logger = logger;
            _placeholderHandler = placeholderHandler;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = _placeholderHandler.GetProductById(id);

            if (product == null)
            {
                return BadRequest();
            }

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string name, string desc)
        {
            var product =  _placeholderHandler.CreateProduct(name, desc);

            if (product == null)
            {
                return BadRequest();
            }

            return Ok(product);
        }

        [HttpPut]
        public async Task<IActionResult> Update(Guid id, string name, string desc)
        {
            _placeholderHandler.UpdateProduct(id, name, desc);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            _placeholderHandler.DeleteProduct(id);

            return Ok();
        }
    }
}
