using HMS.Common.DTOs;
using HMS.Staff.Application.DTOs;
using HMS.Staff.Application.Queries;
using HMS.Staff.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.Staff.Application.Handlers
{
    public class GetStaffByDepartmentQueryHandler : IRequestHandler<GetStaffByDepartmentQuery, Result<List<StaffSummaryDto>>>
    {
        private readonly StaffDbContext _context;
        private readonly ILogger<GetStaffByDepartmentQueryHandler> _logger;

        public GetStaffByDepartmentQueryHandler(StaffDbContext context, ILogger<GetStaffByDepartmentQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<List<StaffSummaryDto>>> Handle(
            GetStaffByDepartmentQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var staff = await _context.Staff
                    .Where(s => s.Department == request.Department && s.IsActive)
                    .OrderBy(s => s.Position)
                    .ThenBy(s => s.StaffNumber)
                    .Select(s => new StaffSummaryDto
                    {
                        Id = s.Id,
                        StaffNumber = s.StaffNumber,
                        FullName = "",
                        Email = "",
                        PhoneNumber = "",
                        ProfilePictureUrl = "",
                        StaffType = s.StaffType.ToString(),
                        Department = s.Department,
                        Position = s.Position,
                        EmploymentStatus = s.EmploymentStatus.ToString(),
                        YearsOfExperience = s.YearsOfExperience
                    })
                    .ToListAsync(cancellationToken);

                return Result<List<StaffSummaryDto>>.Success(staff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving staff by department {Department}", request.Department);
                return Result<List<StaffSummaryDto>>.Failure("Error retrieving staff by department");
            }
        }
    }
}
