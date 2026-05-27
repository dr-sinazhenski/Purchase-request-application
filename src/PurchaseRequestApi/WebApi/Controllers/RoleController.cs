using Application.BusinessLogic.RoleLogic.CreateRole;
using Application.BusinessLogic.RoleLogic.DeleteRole;
using Application.BusinessLogic.RoleLogic.Dto;
using Application.BusinessLogic.RoleLogic.GetAllRoles;
using Application.BusinessLogic.RoleLogic.UpdateRole;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly ILogger<RoleController> _logger;
        private readonly IMediator _mediator;

        public RoleController(ILogger<RoleController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllRolesCommand());
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrudRoleDto dto)
        {
            var result = await _mediator.Send(new CreateRoleCommand(dto));
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CrudRoleDto dto)
        {
            var result = await _mediator.Send(new UpdateRoleCommand(id, dto));
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteRoleCommand(id));
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}