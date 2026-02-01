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
    public class SearchStaffQueryHandler : IRequestHandler<SearchStaffQuery, Result<List<StaffSummaryDto>>>
    {
        private readonly StaffDbContext _context;
        private readonly ILogger<SearchStaffQueryHandler> _logger;

        public SearchStaffQueryHandler(StaffDbContext context, ILogger<SearchStaffQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<List<StaffSummaryDto>>> Handle(
            SearchStaffQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.Staff.AsQueryable();

                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var searchTerm = request.SearchTerm.ToLower();
                    query = query.Where(s =>
                        s.StaffNumber.ToLower().Contains(searchTerm) ||
                        s.Department.ToLower().Contains(searchTerm) ||
                        s.Position.ToLower().Contains(searchTerm) ||
                        (s.Specialization != null && s.Specialization.ToLower().Contains(searchTerm)));
                }

                if (!string.IsNullOrWhiteSpace(request.StaffType))
                {
                    if (Enum.TryParse<StaffType>(request.StaffType, out var staffType))
                    {
                        query = query.Where(s => s.StaffType == staffType);
                    }
                }

                if (!string.IsNullOrWhiteSpace(request.Department))
                {
                    query = query.Where(s => s.Department == request.Department);
                }

                if (!string.IsNullOrWhiteSpace(request.Specialization))
                {
                    query = query.Where(s => s.Specialization == request.Specialization);
                }

                var staff = await query
                    .OrderBy(s => s.StaffNumber)
                    .Take(50) // Limit results
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
                _logger.LogError(ex, "Error searching staff");
                return Result<List<StaffSummaryDto>>.Failure("Error searching staff");
            }
        }
    }
}
