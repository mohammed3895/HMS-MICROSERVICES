using HMS.Staff.Application.Commands;
using HMS.Staff.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HMS.Staff.API.Controllers
{
    [ApiController]
    [Route("api/staff")]
    //[Authorize]
    public class StaffController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<StaffController> _logger;

        public StaffController(IMediator mediator, ILogger<StaffController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Get all staff with pagination and filters
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllStaff(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? staffType = null,
            [FromQuery] string? department = null,
            [FromQuery] string? employmentStatus = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool sortDescending = false)
        {
            var query = new GetAllStaffQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                StaffType = staffType,
                Department = department,
                EmploymentStatus = employmentStatus,
                SortBy = sortBy,
                SortDescending = sortDescending
            };

            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get staff by ID
        /// </summary>
        [HttpGet("{staffId}")]
        public async Task<IActionResult> GetStaffById(Guid staffId)
        {
            var query = new GetStaffByIdQuery { StaffId = staffId };
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Create new staff member
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> CreateStaff([FromBody] CreateStaffCommand command)
        {
            command.CreatedBy = GetCurrentUserId();
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Update staff member
        /// </summary>
        [HttpPut("{staffId}")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> UpdateStaff(Guid staffId, [FromBody] UpdateStaffCommand command)
        {
            command.StaffId = staffId;
            command.UpdatedBy = GetCurrentUserId();
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Update staff status
        /// </summary>
        [HttpPut("{staffId}/status")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> UpdateStaffStatus(Guid staffId, [FromBody] UpdateStaffStatusCommand command)
        {
            command.StaffId = staffId;
            command.UpdatedBy = GetCurrentUserId();
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Delete staff member (soft delete)
        /// </summary>
        [HttpDelete("{staffId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteStaff(Guid staffId)
        {
            var command = new DeleteStaffCommand { StaffId = staffId, DeletedBy = GetCurrentUserId() };
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Search staff members
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchStaff(
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? staffType = null,
            [FromQuery] string? department = null,
            [FromQuery] string? specialization = null)
        {
            var query = new SearchStaffQuery
            {
                SearchTerm = searchTerm,
                StaffType = staffType,
                Department = department,
                Specialization = specialization
            };

            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get staff by type (Doctor, Nurse, etc.)
        /// </summary>
        [HttpGet("type/{staffType}")]
        public async Task<IActionResult> GetStaffByType(string staffType)
        {
            var query = new GetStaffByTypeQuery { StaffType = staffType };
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get staff by department
        /// </summary>
        [HttpGet("department/{department}")]
        public async Task<IActionResult> GetStaffByDepartment(string department)
        {
            var query = new GetStaffByDepartmentQuery { Department = department };
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get staff leaves
        /// </summary>
        [HttpGet("{staffId}/leaves")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> GetStaffLeaves(
            Guid staffId,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] string? status = null)
        {
            var query = new GetStaffLeavesQuery
            {
                StaffId = staffId,
                FromDate = fromDate,
                ToDate = toDate,
                Status = status
            };

            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Apply for leave
        /// </summary>
        [HttpPost("{staffId}/leaves")]
        public async Task<IActionResult> ApplyLeave(Guid staffId, [FromBody] ApplyStaffLeaveCommand command)
        {
            command.StaffId = staffId;
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Approve leave request
        /// </summary>
        [HttpPost("leaves/{leaveId}/approve")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> ApproveLeave(Guid leaveId)
        {
            var command = new ApproveStaffLeaveCommand { LeaveId = leaveId, ApprovedBy = GetCurrentUserId() };
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Reject leave request
        /// </summary>
        [HttpPost("leaves/{leaveId}/reject")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> RejectLeave(Guid leaveId, [FromBody] RejectStaffLeaveCommand command)
        {
            command.LeaveId = leaveId;
            command.RejectedBy = GetCurrentUserId();
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get pending leave requests
        /// </summary>
        [HttpGet("leaves/pending")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> GetPendingLeaveRequests([FromQuery] Guid? staffId = null)
        {
            var query = new GetPendingLeaveRequestsQuery { StaffId = staffId };
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get staff attendance
        /// </summary>
        [HttpGet("{staffId}/attendance")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> GetStaffAttendance(
            Guid staffId,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            var query = new GetStaffAttendanceQuery
            {
                StaffId = staffId,
                FromDate = fromDate,
                ToDate = toDate
            };

            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Record attendance
        /// </summary>
        [HttpPost("{staffId}/attendance")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> RecordAttendance(Guid staffId, [FromBody] RecordAttendanceCommand command)
        {
            command.StaffId = staffId;
            command.CreatedBy = GetCurrentUserId();
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Add education record
        /// </summary>
        [HttpPost("{staffId}/education")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> AddEducation(Guid staffId, [FromBody] AddStaffEducationCommand command)
        {
            command.StaffId = staffId;
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Add performance review
        /// </summary>
        [HttpPost("{staffId}/reviews")]
        [Authorize(Roles = "Admin,HR,Manager")]
        public async Task<IActionResult> AddPerformanceReview(Guid staffId, [FromBody] AddPerformanceReviewCommand command)
        {
            command.StaffId = staffId;
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get staff dashboard data
        /// </summary>
        [HttpGet("{staffId}/dashboard")]
        public async Task<IActionResult> GetStaffDashboard(Guid staffId)
        {
            var query = new GetStaffDashboardQuery { StaffId = staffId };
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get department statistics
        /// </summary>
        [HttpGet("departments/{department}/statistics")]
        [Authorize(Roles = "Admin,HR,Manager")]
        public async Task<IActionResult> GetDepartmentStatistics(string department)
        {
            var query = new GetDepartmentStatisticsQuery { Department = department };
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("user_id")?.Value
                ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("User ID not found in token");

            return userId;
        }
    }
}