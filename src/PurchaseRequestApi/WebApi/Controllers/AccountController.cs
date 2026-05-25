using Application.BusinessLogic.AccountLogic.CreateAccount;
using Application.BusinessLogic.AccountLogic.DeleteAccount;
using Application.BusinessLogic.AccountLogic.Dto;
using Application.BusinessLogic.AccountLogic.GetAllAccounts;
using Application.BusinessLogic.AccountLogic.UpdateAccount;
using Application.BusinessLogic.AccountLogic.Login;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IMediator _mediator;

        public AccountController(ILogger<AccountController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllAccountsCommand());

            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to fetch accounts");
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAccountDto dto)
        {
            var result = await _mediator.Send(new CreateAccountCommand(dto));

            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to create account");
                return BadRequest(result);
            }

            _logger.LogInformation("Account creation succeeded");
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateAccountDto dto)
        {
            var result = await _mediator.Send(new UpdateAccountCommand(dto));

            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to update account with Id={Id}", dto.Id);
                return BadRequest(result);
            }

            _logger.LogInformation("Account update succeeded for Id={Id}", dto.Id);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteAccountCommand(id));

            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to delete account with Id={Id}", id);
                return BadRequest(result);
            }

            _logger.LogInformation("Account deletion succeeded for Id={Id}", id);
            return Ok(result);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _mediator.Send(new LoginCommand(dto));
            if (!result.IsSuccess)
            {
                _logger.LogError("Login failed for {Login}", dto.Login);
                return Unauthorized(result);
            }
            return Ok(result);
        }
    }
}