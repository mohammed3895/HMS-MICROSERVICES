using HMS.Authentication.Application.Commands.Admin;
using HMS.Authentication.Application.Commands.Authentication;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HMS.Authentication.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(
            IMediator mediator,
            ILogger<AuthenticationController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Register a new patient account
        /// </summary>
        /// <remarks>
        /// Patients can self-register. An OTP will be sent to their email for verification.
        /// </remarks>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Confirm email address with OTP
        /// </summary>
        [HttpPost("confirm-email")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Resend OTP for email verification or login
        /// </summary>
        [HttpPost("resend-otp")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : StatusCode(429, result);
        }

        /// <summary>
        /// Login to the system
        /// </summary>
        /// <remarks>
        /// Returns tokens if login is successful. May require OTP or 2FA based on security settings.
        /// </remarks>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : Unauthorized(result);
        }

        /// <summary>
        /// Verify OTP code for login
        /// </summary>
        /// <remarks>
        /// Required when logging in from a new device or when OTP verification is enabled.
        /// </remarks>
        [HttpPost("verify-login-otp")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> VerifyLoginOtp([FromBody] VerifyLoginOtpCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Refresh access token using refresh token
        /// </summary>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : Unauthorized(result);
        }

        /// <summary>
        /// Request password reset - sends reset token via email
        /// </summary>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ForgotPassword([FromBody] RequestPasswordResetCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result); // Always return OK to prevent email enumeration
        }

        /// <summary>
        /// Reset password using token from email
        /// </summary>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        // ==================== Authenticated User Endpoints ====================

        /// <summary>
        /// Logout from the system
        /// </summary>
        /// <remarks>
        /// Can logout from current device or all devices.
        /// </remarks>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout([FromBody] LogoutCommand command)
        {
            var userId = GetCurrentUserId();
            command.UserId = userId;

            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Change password for authenticated user
        /// </summary>
        [HttpPost("change-password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            var userId = GetCurrentUserId();
            command.UserId = userId;

            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Enable two-factor authentication
        /// </summary>
        [HttpPost("2fa/enable")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Enable2FA([FromBody] Enable2FACommand command)
        {
            var userId = GetCurrentUserId();
            command.UserId = userId;

            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Verify 2FA setup with code
        /// </summary>
        [HttpPost("2fa/verify-setup")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Verify2FASetup([FromBody] Verify2FASetupCommand command)
        {
            var userId = GetCurrentUserId();
            command.UserId = userId;

            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Verify 2FA code during login
        /// </summary>
        /// <remarks>
        /// Required when logging in with 2FA enabled.
        /// </remarks>
        [HttpPost("2fa/verify-code")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Verify2FACode([FromBody] Verify2FACodeCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Use recovery code for 2FA login
        /// </summary>
        [HttpPost("2fa/use-recovery-code")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UseRecoveryCode([FromBody] UseRecoveryCodeCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Disable two-factor authentication
        /// </summary>
        [HttpPost("2fa/disable")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Disable2FA([FromBody] Disable2FACommand command)
        {
            var userId = GetCurrentUserId();
            command.UserId = userId;

            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Generate new recovery codes for 2FA
        /// </summary>
        [HttpPost("2fa/generate-recovery-codes")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GenerateRecoveryCodes([FromBody] GenerateRecoveryCodesCommand command)
        {
            var userId = GetCurrentUserId();
            command.UserId = userId;

            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        // ==================== Admin/Manager Only Endpoints ====================

        /// <summary>
        /// Create a new staff member account (Admin/Manager only)
        /// </summary>
        /// <remarks>
        /// Only administrators and managers can create accounts for doctors, nurses, and other staff.
        /// </remarks>
        [HttpPost("staff/create")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateStaff([FromBody] CreateStaffCommand command)
        {
            var adminId = GetCurrentUserId();
            command.CreatedBy = adminId;

            _logger.LogInformation("Staff creation initiated by admin {AdminId}: Role={Role}",
                adminId, command.Role);

            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get list of all staff members (Admin/Manager only)
        /// </summary>
        [HttpGet("staff/list")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetStaffList()
        {
            // This would be implemented with a separate query handler
            return Ok(new { message = "Staff list endpoint" });
        }

        /// <summary>
        /// Deactivate a user account (Admin only)
        /// </summary>
        [HttpPost("staff/{userId}/deactivate")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeactivateUser(Guid userId)
        {
            // This would be implemented with a separate command handler
            return Ok(new { message = $"User {userId} deactivation endpoint" });
        }

        /// <summary>
        /// View audit logs (Admin only)
        /// </summary>
        [HttpGet("audit-logs")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAuditLogs([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            // This would be implemented with a separate query handler
            return Ok(new { message = "Audit logs endpoint" });
        }

        /// <summary>
        /// View security events (Admin only)
        /// </summary>
        [HttpGet("security-events")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetSecurityEvents([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            // This would be implemented with a separate query handler
            return Ok(new { message = "Security events endpoint" });
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

        private bool IsInRole(string role)
        {
            return User.IsInRole(role);
        }

        private List<string> GetUserRoles()
        {
            return User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();
        }
    }
}