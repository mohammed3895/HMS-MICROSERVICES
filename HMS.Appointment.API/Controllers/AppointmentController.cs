using HMS.Appointment.Application.Commands;
using HMS.Appointment.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HMS.Appointment.API.Controllers
{
    [ApiController]
    [Route("api/appointments")]
    [Authorize]
    public class AppointmentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AppointmentController> _logger;

        public AppointmentController(
            IMediator mediator,
            ILogger<AppointmentController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Create a new appointment
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentCommand command)
        {
            command.CreatedBy = GetCurrentUserId();
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get available time slots for a doctor on a specific date
        /// </summary>
        [HttpGet("available-slots")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAvailableTimeSlots(
            [FromQuery] Guid doctorId,
            [FromQuery] DateTime date)
        {
            var query = new GetAvailableTimeSlotsQuery
            {
                DoctorId = doctorId,
                Date = date
            };
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get appointment by ID
        /// </summary>
        [HttpGet("{appointmentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAppointmentById(Guid appointmentId)
        {
            // Implementation would use a query handler
            return Ok(new { message = $"Get appointment {appointmentId}" });
        }

        /// <summary>
        /// Get patient's appointments
        /// </summary>
        [HttpGet("patient/{patientId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPatientAppointments(
            Guid patientId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] string? status)
        {
            var query = new GetPatientAppointmentsQuery
            {
                PatientId = patientId,
                FromDate = fromDate,
                ToDate = toDate,
                Status = status
            };
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get doctor's appointments for a specific date
        /// </summary>
        [HttpGet("doctor/{doctorId}/daily")]
        [Authorize(Roles = "Doctor,Nurse,Receptionist,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDoctorDailyAppointments(
            Guid doctorId,
            [FromQuery] DateTime date)
        {
            var query = new GetDoctorAppointmentsQuery
            {
                DoctorId = doctorId,
                Date = date
            };
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Reschedule an appointment
        /// </summary>
        [HttpPut("{appointmentId}/reschedule")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RescheduleAppointment(
            Guid appointmentId,
            [FromBody] RescheduleAppointmentCommand command)
        {
            command.AppointmentId = appointmentId;
            command.RescheduledBy = GetCurrentUserId();
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Cancel an appointment
        /// </summary>
        [HttpPut("{appointmentId}/cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CancelAppointment(
            Guid appointmentId,
            [FromBody] CancelAppointmentCommand command)
        {
            command.AppointmentId = appointmentId;
            command.CancelledBy = GetCurrentUserId();
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Check-in for an appointment
        /// </summary>
        [HttpPost("{appointmentId}/check-in")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CheckIn(
            Guid appointmentId,
            [FromBody] CheckInAppointmentCommand command)
        {
            command.AppointmentId = appointmentId;
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Start consultation (Doctor only)
        /// </summary>
        [HttpPost("{appointmentId}/start-consultation")]
        [Authorize(Roles = "Doctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> StartConsultation(
            Guid appointmentId,
            [FromBody] StartConsultationCommand command)
        {
            command.AppointmentId = appointmentId;
            command.DoctorId = GetCurrentUserId();
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Complete an appointment (Doctor only)
        /// </summary>
        [HttpPost("{appointmentId}/complete")]
        [Authorize(Roles = "Doctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CompleteAppointment(
            Guid appointmentId,
            [FromBody] CompleteAppointmentCommand command)
        {
            command.AppointmentId = appointmentId;
            command.CompletedBy = GetCurrentUserId();
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Mark appointment as no-show
        /// </summary>
        [HttpPost("{appointmentId}/no-show")]
        [Authorize(Roles = "Receptionist,Doctor,Nurse,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> MarkNoShow(
            Guid appointmentId,
            [FromBody] MarkNoShowCommand command)
        {
            command.AppointmentId = appointmentId;
            command.MarkedBy = GetCurrentUserId();
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Confirm appointment
        /// </summary>
        [HttpPost("{appointmentId}/confirm")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ConfirmAppointment(
            Guid appointmentId,
            [FromBody] ConfirmAppointmentCommand command)
        {
            command.AppointmentId = appointmentId;
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Add patient to waitlist
        /// </summary>
        [HttpPost("waitlist")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddToWaitlist([FromBody] AddToWaitlistCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Send appointment reminder
        /// </summary>
        [HttpPost("{appointmentId}/send-reminder")]
        [Authorize(Roles = "Receptionist,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SendReminder(
            Guid appointmentId,
            [FromBody] SendAppointmentReminderCommand command)
        {
            command.AppointmentId = appointmentId;
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get appointment history
        /// </summary>
        [HttpGet("{appointmentId}/history")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAppointmentHistory(Guid appointmentId)
        {
            // Implementation would use a query handler
            return Ok(new { message = $"Get history for appointment {appointmentId}" });
        }

        /// <summary>
        /// Get today's appointments dashboard
        /// </summary>
        [HttpGet("today/dashboard")]
        [Authorize(Roles = "Doctor,Nurse,Receptionist,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTodaysDashboard()
        {
            // Implementation would use a query handler
            return Ok(new { message = "Today's dashboard" });
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