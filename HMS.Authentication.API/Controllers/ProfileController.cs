using HMS.Authentication.Application.Commands.Profile;
using HMS.Authentication.Application.Queries.Profile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HMS.Authentication.API.Controllers
{
    [ApiController]
    [Route("api/profile")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(
            IMediator mediator,
            ILogger<ProfileController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Get current user's complete profile including role-specific data
        /// </summary>
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = GetCurrentUserId();
            var query = new Application.Queries.Profile.GetUserProfileQuery { UserId = userId };
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get user profile by ID (Admin/Manager can view any profile)
        /// </summary>
        [HttpGet("{userId}")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserProfile(Guid userId)
        {
            var query = new Application.Queries.Profile.GetUserProfileQuery { UserId = userId };
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Update basic user profile information
        /// </summary>
        [HttpPut("basic")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateBasicProfile([FromBody] UpdateBasicProfileCommand command)
        {
            var userId = GetCurrentUserId();
            command.UserId = userId;
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Update user contact information
        /// </summary>
        [HttpPut("contact")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateContactInfo([FromBody] UpdateContactInfoCommand command)
        {
            var userId = GetCurrentUserId();
            command.UserId = userId;
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Update profile picture
        /// </summary>
        [HttpPut("picture")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProfilePicture(IFormFile file)
        {
            var userId = GetCurrentUserId();
            var command = new UpdateProfilePictureCommand
            {
                UserId = userId,
                File = file
            };
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        // ==================== Role-Specific Profile Endpoints ====================

        /// <summary>
        /// Get doctor-specific profile (Doctor role only)
        /// </summary>
        [HttpGet("doctor")]
        [Authorize(Roles = "Doctor,Admin,Manager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetDoctorProfile()
        {
            var userId = GetCurrentUserId();
            var query = new GetDoctorProfileQuery { UserId = userId };
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Update doctor-specific profile
        /// </summary>
        [HttpPut("doctor")]
        [Authorize(Roles = "Doctor,Admin,Manager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateDoctorProfile([FromBody] UpdateDoctorProfileCommand command)
        {
            var userId = GetCurrentUserId();
            command.UserId = userId;
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get nurse-specific profile
        /// </summary>
        [HttpGet("nurse")]
        [Authorize(Roles = "Nurse,Admin,Manager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNurseProfile()
        {
            var userId = GetCurrentUserId();
            var query = new GetNurseProfileQuery { UserId = userId };
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Update nurse-specific profile
        /// </summary>
        [HttpPut("nurse")]
        [Authorize(Roles = "Nurse,Admin,Manager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateNurseProfile([FromBody] UpdateNurseProfileCommand command)
        {
            var userId = GetCurrentUserId();
            command.UserId = userId;
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get pharmacist-specific profile
        /// </summary>
        [HttpGet("pharmacist")]
        [Authorize(Roles = "Pharmacist,Admin,Manager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPharmacistProfile()
        {
            var userId = GetCurrentUserId();
            var query = new GetPharmacistProfileQuery { UserId = userId };
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Update pharmacist-specific profile
        /// </summary>
        [HttpPut("pharmacist")]
        [Authorize(Roles = "Pharmacist,Admin,Manager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatePharmacistProfile([FromBody] UpdatePharmacistProfileCommand command)
        {
            var userId = GetCurrentUserId();
            command.UserId = userId;
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        // ==================== Settings Endpoints ====================

        /// <summary>
        /// Get user settings and preferences
        /// </summary>
        [HttpGet("settings")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserSettings()
        {
            var userId = GetCurrentUserId();
            var query = new GetUserSettingsQuery { UserId = userId };
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Update user settings and preferences
        /// </summary>
        [HttpPut("settings")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUserSettings([FromBody] UpdateUserSettingsCommand command)
        {
            var userId = GetCurrentUserId();
            command.UserId = userId;
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Update notification preferences
        /// </summary>
        [HttpPut("settings/notifications")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateNotificationSettings([FromBody] UpdateNotificationSettingsCommand command)
        {
            var userId = GetCurrentUserId();
            command.UserId = userId;
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Update privacy settings
        /// </summary>
        [HttpPut("settings/privacy")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatePrivacySettings([FromBody] UpdatePrivacySettingsCommand command)
        {
            var userId = GetCurrentUserId();
            command.UserId = userId;
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