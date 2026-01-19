using HMS.Authentication.Application.Commands.Authentication.WebAuth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HMS.Authentication.API.Controllers
{
    [ApiController]
    [Route("api/auth/webauthn")]
    public class WebAuthnController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<WebAuthnController> _logger;

        public WebAuthnController(
            IMediator mediator,
            ILogger<WebAuthnController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Begin WebAuthn registration - generates challenge and options
        /// </summary>
        /// <remarks>
        /// User must be authenticated to register a new WebAuthn credential.
        /// Returns options needed by the browser's WebAuthn API (navigator.credentials.create).
        /// </remarks>
        [HttpPost("register/begin")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> BeginRegistration()
        {
            var userId = GetCurrentUserId();

            var command = new BeginWebAuthnRegistrationCommand
            {
                UserId = userId
            };

            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Complete WebAuthn registration - verify and store credential
        /// </summary>
        /// <remarks>
        /// Completes the registration by verifying the attestation from the authenticator
        /// and storing the public key for future authentication.
        /// The credential data comes from navigator.credentials.create() response.
        /// </remarks>
        [HttpPost("register/complete")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CompleteRegistration(
            [FromBody] CompleteWebAuthnRegistrationCommand command)
        {
            var userId = GetCurrentUserId();
            command.UserId = userId;

            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Begin WebAuthn authentication - generates challenge
        /// </summary>
        /// <remarks>
        /// Public endpoint that initiates WebAuthn authentication.
        /// Returns challenge and list of allowed credentials for the user.
        /// The client should call navigator.credentials.get() with these options.
        /// </remarks>
        [HttpPost("authenticate/begin")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> BeginAuthentication(
            [FromBody] BeginWebAuthnAuthenticationCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Complete WebAuthn authentication - verify and issue tokens
        /// </summary>
        /// <remarks>
        /// Completes authentication by verifying the assertion signature from navigator.credentials.get()
        /// and issues JWT access and refresh tokens on success.
        /// Includes signature counter validation to detect cloned credentials.
        /// </remarks>
        [HttpPost("authenticate/complete")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CompleteAuthentication(
            [FromBody] CompleteWebAuthnAuthenticationCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : Unauthorized(result);
        }

        /// <summary>
        /// List all WebAuthn credentials for the authenticated user
        /// </summary>
        /// <remarks>
        /// Returns all registered WebAuthn credentials (security keys, biometrics, etc.)
        /// for the current user. Shows device names, creation dates, usage statistics,
        /// and backup status.
        /// </remarks>
        [HttpGet("credentials")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ListCredentials()
        {
            var userId = GetCurrentUserId();

            var command = new ListWebAuthnCredentialsCommand
            {
                UserId = userId
            };

            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Remove (revoke) a WebAuthn credential
        /// </summary>
        /// <remarks>
        /// Allows users to remove a registered security key or biometric credential.
        /// The credential is soft-deleted (marked as revoked) for audit purposes.
        /// </remarks>
        [HttpDelete("credentials/{credentialId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveCredential(Guid credentialId)
        {
            var userId = GetCurrentUserId();

            var command = new RemoveWebAuthnCredentialCommand
            {
                UserId = userId,
                CredentialId = credentialId
            };

            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        // ==================== Helper Methods ====================

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("user_id")?.Value
                ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("User ID not found in token");
            }

            return userId;
        }
    }
}