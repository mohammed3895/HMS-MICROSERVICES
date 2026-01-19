using HMS.Authentication.Application.Commands.Roles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Authentication.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "RequireAdminRole")]
    public class RolesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RolesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess
                ? CreatedAtAction(nameof(CreateRole), new { id = result.Data?.RoleId }, result)
                : BadRequest(result);
        }

        [HttpPut("{roleId}")]
        public async Task<IActionResult> UpdateRole(Guid roleId, [FromBody] UpdateRoleCommand command)
        {
            if (roleId != command.RoleId)
                return BadRequest("Role ID mismatch");

            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{roleId}")]
        public async Task<IActionResult> DeleteRole(Guid roleId)
        {
            var command = new DeleteRoleCommand { RoleId = roleId };
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{roleId}/permissions")]
        public async Task<IActionResult> GrantPermission(Guid roleId, [FromBody] GrantPermissionCommand command)
        {
            if (roleId != command.RoleId)
                return BadRequest("Role ID mismatch");

            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{roleId}/permissions")]
        public async Task<IActionResult> RevokePermission(Guid roleId, [FromBody] RevokePermissionCommand command)
        {
            if (roleId != command.RoleId)
                return BadRequest("Role ID mismatch");

            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("remove")]
        public async Task<IActionResult> RemoveRole([FromBody] RemoveRoleCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}