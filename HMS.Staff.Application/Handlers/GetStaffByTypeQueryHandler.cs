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
    public class GetStaffByTypeQueryHandler : IRequestHandler<GetStaffByTypeQuery, Result<List<StaffSummaryDto>>>
    {
        private readonly StaffDbContext _context;
        private readonly ILogger<GetStaffByTypeQueryHandler> _logger;

        public GetStaffByTypeQueryHandler(StaffDbContext context, ILogger<GetStaffByTypeQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<List<StaffSummaryDto>>> Handle(
            GetStaffByTypeQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (!Enum.TryParse<StaffType>(request.StaffType, out var staffType))
                {
                    return Result<List<StaffSummaryDto>>.Failure("Invalid staff type");
                }

                var staff = await _context.Staff
                    .Where(s => s.StaffType == staffType && s.IsActive)
                    .OrderBy(s => s.StaffNumber)
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
                _logger.LogError(ex, "Error retrieving staff by type {StaffType}", request.StaffType);
                return Result<List<StaffSummaryDto>>.Failure("Error retrieving staff by type");
            }
        }
    }
}
