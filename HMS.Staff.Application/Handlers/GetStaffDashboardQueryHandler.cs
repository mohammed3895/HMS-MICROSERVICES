using HMS.Common.DTOs;
using HMS.Staff.Application.DTOs;
using HMS.Staff.Application.Interfaces;
using HMS.Staff.Application.Queries;
using HMS.Staff.Domain.Enums;
using HMS.Staff.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.Staff.Application.Handlers
{
    public class GetStaffDashboardQueryHandler : IRequestHandler<GetStaffDashboardQuery, Result<StaffDashboardDto>>
    {
        private readonly StaffDbContext _context;
        private readonly IAuthServiceClient _authServiceClient;
        private readonly ILogger<GetStaffDashboardQueryHandler> _logger;

        public GetStaffDashboardQueryHandler(
            StaffDbContext context,
            IAuthServiceClient authServiceClient,
            ILogger<GetStaffDashboardQueryHandler> logger)
        {
            _context = context;
            _authServiceClient = authServiceClient;
            _logger = logger;
        }

        public async Task<Result<StaffDashboardDto>> Handle(
            GetStaffDashboardQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var staff = await _context.Staff
                    .FirstOrDefaultAsync(s => s.Id == request.StaffId, cancellationToken);

                if (staff == null)
                {
                    return Result<StaffDashboardDto>.Failure("Staff member not found");
                }

                // Fetch user info
                var userInfo = await _authServiceClient.GetUserInfoAsync(staff.UserId);

                var currentMonth = DateTime.UtcNow.Month;
                var currentYear = DateTime.UtcNow.Year;

                var attendanceRecords = await _context.StaffAttendances
                    .Where(a => a.StaffId == request.StaffId &&
                               a.Date.Month == currentMonth &&
                               a.Date.Year == currentYear)
                    .ToListAsync(cancellationToken);

                var totalWorkingDays = attendanceRecords.Count;
                var presentDays = attendanceRecords.Count(a => a.Status == AttendanceStatus.Present);
                var absentDays = attendanceRecords.Count(a => a.Status == AttendanceStatus.Absent);

                var pendingLeaves = await _context.StaffLeaves
                    .CountAsync(l => l.StaffId == request.StaffId &&
                                    l.Status == LeaveStatus.Pending,
                               cancellationToken);

                var attendancePercentage = totalWorkingDays > 0
                    ? (decimal)presentDays / totalWorkingDays * 100
                    : 0;

                var dashboard = new StaffDashboardDto
                {
                    StaffInfo = new StaffSummaryDto
                    {
                        Id = staff.Id,
                        StaffNumber = staff.StaffNumber,
                        FullName = userInfo != null ? $"{userInfo.FirstName} {userInfo.LastName}" : "N/A",
                        Email = userInfo?.Email ?? "",
                        PhoneNumber = userInfo?.PhoneNumber ?? "",
                        ProfilePictureUrl = userInfo?.ProfilePictureUrl ?? "",
                        StaffType = staff.StaffType.ToString(),
                        Department = staff.Department,
                        Position = staff.Position,
                        EmploymentStatus = staff.EmploymentStatus.ToString(),
                        YearsOfExperience = staff.YearsOfExperience
                    },
                    TotalWorkingDays = totalWorkingDays,
                    PresentDays = presentDays,
                    AbsentDays = absentDays,
                    PendingLeaveRequests = pendingLeaves,
                    AttendancePercentage = attendancePercentage
                };

                return Result<StaffDashboardDto>.Success(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard for {StaffId}", request.StaffId);
                return Result<StaffDashboardDto>.Failure("Error retrieving dashboard");
            }
        }
    }
}
