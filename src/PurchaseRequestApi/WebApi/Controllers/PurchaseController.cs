using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public abstract class PurchaseController : Controller
    {
        protected readonly ILogger _logger;
        protected readonly IMediator _mediator;

        protected PurchaseController(ILogger logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }
    }
}
