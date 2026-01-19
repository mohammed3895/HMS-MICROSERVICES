using HMS.Authentication.Application.Commands.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Authentication.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess
                ? CreatedAtAction(nameof(CreateUser), new { id = result.Data?.UserId }, result)
                : BadRequest(result);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserCommand command)
        {
            if (userId != command.UserId)
                return BadRequest("User ID mismatch");

            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{userId}/profile")]
        public async Task<IActionResult> UpdateUserProfile(Guid userId, [FromBody] UpdateUserProfileCommand command)
        {
            if (userId != command.UserId)
                return BadRequest("User ID mismatch");

            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{userId}/activate")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> ActivateUser(Guid userId)
        {
            var command = new ActivateUserCommand { UserId = userId };
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{userId}/deactivate")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> DeactivateUser(Guid userId)
        {
            var command = new DeactivateUserCommand { UserId = userId };
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}