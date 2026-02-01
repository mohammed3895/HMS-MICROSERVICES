using HMS.Common.DTOs;
using HMS.Staff.Application.DTOs;
using HMS.Staff.Application.Queries;
using HMS.Staff.Domain.Enums;
using HMS.Staff.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.Staff.Application.Handlers
{
    public class GetDepartmentStatisticsQueryHandler : IRequestHandler<GetDepartmentStatisticsQuery, Result<DepartmentStatisticsDto>>
    {
        private readonly StaffDbContext _context;
        private readonly ILogger<GetDepartmentStatisticsQueryHandler> _logger;

        public GetDepartmentStatisticsQueryHandler(StaffDbContext context, ILogger<GetDepartmentStatisticsQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<DepartmentStatisticsDto>> Handle(
            GetDepartmentStatisticsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var departmentStaff = await _context.Staff
                    .Where(s => s.Department == request.Department)
                    .ToListAsync(cancellationToken);

                var totalStaff = departmentStaff.Count;
                var activeStaff = departmentStaff.Count(s => s.EmploymentStatus == EmploymentStatus.Active);

                var currentDate = DateTime.Today;
                var onLeaveStaffIds = await _context.StaffLeaves
                    .Where(l => l.Status == LeaveStatus.Approved &&
                               l.StartDate <= currentDate &&
                               l.EndDate >= currentDate)
                    .Select(l => l.StaffId)
                    .ToListAsync(cancellationToken);

                var onLeaveStaff = departmentStaff.Count(s => onLeaveStaffIds.Contains(s.Id));

                var staffByType = departmentStaff
                    .GroupBy(s => s.StaffType.ToString())
                    .ToDictionary(g => g.Key, g => g.Count());

                var statistics = new DepartmentStatisticsDto
                {
                    Department = request.Department,
                    TotalStaff = totalStaff,
                    ActiveStaff = activeStaff,
                    OnLeaveStaff = onLeaveStaff,
                    StaffByType = staffByType
                };

                return Result<DepartmentStatisticsDto>.Success(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving statistics for department {Department}", request.Department);
                return Result<DepartmentStatisticsDto>.Failure("Error retrieving department statistics");
            }
        }
    }
}
