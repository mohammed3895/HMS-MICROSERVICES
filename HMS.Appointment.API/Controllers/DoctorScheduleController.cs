using HMS.Appointment.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HMS.Appointment.API.Controllers
{
    [ApiController]
    [Route("api/doctor-schedule")]
    [Authorize]
    public class DoctorScheduleController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DoctorScheduleController> _logger;

        public DoctorScheduleController(
            IMediator mediator,
            ILogger<DoctorScheduleController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Get doctor's schedule
        /// </summary>
        [HttpGet("{doctorId}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDoctorSchedule(Guid doctorId)
        {
            // Implementation would use a query handler
            return Ok(new { message = $"Get schedule for doctor {doctorId}" });
        }

        /// <summary>
        /// Update doctor's schedule (Admin/Doctor only)
        /// </summary>
        [HttpPut("{doctorId}")]
        [Authorize(Roles = "Admin,Doctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateDoctorSchedule(
            Guid doctorId,
            [FromBody] UpdateDoctorScheduleCommand command)
        {
            command.DoctorId = doctorId;
            command.UpdatedBy = GetCurrentUserId();
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Apply for leave (Doctor only)
        /// </summary>
        [HttpPost("{doctorId}/leave")]
        [Authorize(Roles = "Doctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ApplyLeave(
            Guid doctorId,
            [FromBody] ApplyDoctorLeaveCommand command)
        {
            command.DoctorId = doctorId;
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get doctor's leave history
        /// </summary>
        [HttpGet("{doctorId}/leave")]
        [Authorize(Roles = "Doctor,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDoctorLeaves(
            Guid doctorId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            // Implementation would use a query handler
            return Ok(new { message = $"Get leaves for doctor {doctorId}" });
        }

        /// <summary>
        /// Approve doctor leave (Admin only)
        /// </summary>
        [HttpPost("leave/{leaveId}/approve")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ApproveLeave(Guid leaveId)
        {
            // Implementation would use a command handler
            return Ok(new { message = $"Approve leave {leaveId}" });
        }

        /// <summary>
        /// Reject doctor leave (Admin only)
        /// </summary>
        [HttpPost("leave/{leaveId}/reject")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> RejectLeave(
            Guid leaveId,
            [FromBody] RejectLeaveRequest request)
        {
            // Implementation would use a command handler
            return Ok(new { message = $"Reject leave {leaveId}" });
        }

        /// <summary>
        /// Cancel doctor leave
        /// </summary>
        [HttpPost("leave/{leaveId}/cancel")]
        [Authorize(Roles = "Doctor,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CancelLeave(Guid leaveId)
        {
            // Implementation would use a command handler
            return Ok(new { message = $"Cancel leave {leaveId}" });
        }

        /// <summary>
        /// Bulk reschedule appointments (when doctor on leave)
        /// </summary>
        [HttpPost("{doctorId}/bulk-reschedule")]
        [Authorize(Roles = "Admin,Receptionist")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> BulkRescheduleAppointments(
            Guid doctorId,
            [FromBody] BulkRescheduleAppointmentsCommand command)
        {
            command.DoctorId = doctorId;
            command.RescheduledBy = GetCurrentUserId();
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Add schedule exception (one-time change)
        /// </summary>
        [HttpPost("{doctorId}/exception")]
        [Authorize(Roles = "Doctor,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddScheduleException(
            Guid doctorId,
            [FromBody] AddScheduleExceptionRequest request)
        {
            // Implementation would use a command handler
            return Ok(new { message = $"Add exception for doctor {doctorId}" });
        }

        /// <summary>
        /// Get doctor availability for date range
        /// </summary>
        [HttpGet("{doctorId}/availability")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDoctorAvailability(
            Guid doctorId,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
        {
            // Implementation would use a query handler
            return Ok(new { message = $"Get availability for doctor {doctorId}" });
        }

        /// <summary>
        /// Block time slots (for meetings, procedures, etc.)
        /// </summary>
        [HttpPost("{doctorId}/block-slots")]
        [Authorize(Roles = "Doctor,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> BlockTimeSlots(
            Guid doctorId,
            [FromBody] BlockTimeSlotsRequest request)
        {
            // Implementation would use a command handler
            return Ok(new { message = $"Block slots for doctor {doctorId}" });
        }

        /// <summary>
        /// Get doctor workload statistics
        /// </summary>
        [HttpGet("{doctorId}/workload")]
        [Authorize(Roles = "Doctor,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDoctorWorkload(
            Guid doctorId,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
        {
            // Implementation would use a query handler
            return Ok(new { message = $"Get workload for doctor {doctorId}" });
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

    // Request DTOs
    public class RejectLeaveRequest
    {
        public string Reason { get; set; } = string.Empty;
    }

    public class AddScheduleExceptionRequest
    {
        public DateTime Date { get; set; }
        public TimeSpan? NewStartTime { get; set; }
        public TimeSpan? NewEndTime { get; set; }
        public bool IsClosed { get; set; }
        public string? Reason { get; set; }
    }

    public class BlockTimeSlotsRequest
    {
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}