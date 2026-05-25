using Application.BusinessLogic.ApproverProfileLogic.CreateApproverProfile;
using Application.BusinessLogic.ApproverProfileLogic.DeleteApproverProfile;
using Application.BusinessLogic.ApproverProfileLogic.Dto;
using Application.BusinessLogic.ApproverProfileLogic.GetAllApproverProfiles;
using Application.BusinessLogic.ApproverProfileLogic.UpdateApproverProfile;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApproverProfileController : ControllerBase
    {
        private readonly ILogger<ApproverProfileController> _logger;
        private readonly IMediator _mediator;

        public ApproverProfileController(ILogger<ApproverProfileController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllApproverProfilesCommand());

            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to fetch approver profiles");
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrudApproverProfileDto dto)
        {
            var result = await _mediator.Send(new CreateApproverProfileCommand(dto));

            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to create approver profile");
                return BadRequest(result);
            }

            _logger.LogInformation("Approver profile creation succeeded");
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CrudApproverProfileDto dto)
        {
            var result = await _mediator.Send(new UpdateApproverProfileCommand(id, dto));

            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to update approver profile with Id={Id}", id);
                return BadRequest(result);
            }

            _logger.LogInformation("Approver profile update succeeded for Id={Id}", id);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteApproverProfileCommand(id));

            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to delete approver profile with Id={Id}", id);
                return BadRequest(result);
            }

            _logger.LogInformation("Approver profile deletion succeeded for Id={Id}", id);
            return Ok(result);
        }
    }
}