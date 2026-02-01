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
    public class GetAllStaffQueryHandler : IRequestHandler<GetAllStaffQuery, Result<PaginatedList<StaffSummaryDto>>>
    {
        private readonly StaffDbContext _context;
        private readonly IAuthServiceClient _authServiceClient;
        private readonly ILogger<GetAllStaffQueryHandler> _logger;

        public GetAllStaffQueryHandler(
            StaffDbContext context,
            IAuthServiceClient authServiceClient,
            ILogger<GetAllStaffQueryHandler> logger)
        {
            _context = context;
            _authServiceClient = authServiceClient;
            _logger = logger;
        }

        public async Task<Result<PaginatedList<StaffSummaryDto>>> Handle(
            GetAllStaffQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.Staff.AsQueryable();

                // Apply filters
                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var searchTerm = request.SearchTerm.ToLower();
                    query = query.Where(s =>
                        s.StaffNumber.ToLower().Contains(searchTerm) ||
                        s.Department.ToLower().Contains(searchTerm) ||
                        s.Position.ToLower().Contains(searchTerm));
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

                if (!string.IsNullOrWhiteSpace(request.EmploymentStatus))
                {
                    if (Enum.TryParse<EmploymentStatus>(request.EmploymentStatus, out var status))
                    {
                        query = query.Where(s => s.EmploymentStatus == status);
                    }
                }

                // Apply sorting
                query = request.SortBy?.ToLower() switch
                {
                    "staffnumber" => request.SortDescending
                        ? query.OrderByDescending(s => s.StaffNumber)
                        : query.OrderBy(s => s.StaffNumber),
                    "department" => request.SortDescending
                        ? query.OrderByDescending(s => s.Department)
                        : query.OrderBy(s => s.Department),
                    "position" => request.SortDescending
                        ? query.OrderByDescending(s => s.Position)
                        : query.OrderBy(s => s.Position),
                    "joindate" => request.SortDescending
                        ? query.OrderByDescending(s => s.JoinDate)
                        : query.OrderBy(s => s.JoinDate),
                    _ => query.OrderBy(s => s.StaffNumber)
                };

                // Get total count
                var totalCount = await query.CountAsync(cancellationToken);

                // Apply pagination
                var staffList = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync(cancellationToken);

                // Fetch user info from Auth service
                var userIds = staffList.Select(s => s.UserId).Where(id => id != Guid.Empty).ToList();
                var usersInfo = await _authServiceClient.GetUsersInfoAsync(userIds);
                var userDict = usersInfo.ToDictionary(u => u.UserId, u => u);

                // Map to DTOs with user info
                var items = staffList.Select(s =>
                {
                    userDict.TryGetValue(s.UserId, out var userInfo);
                    return new StaffSummaryDto
                    {
                        Id = s.Id,
                        UserId = s.UserId.ToString(),
                        StaffNumber = s.StaffNumber,
                        FullName = userInfo != null ? $"{userInfo.FirstName} {userInfo.LastName}" : "N/A",
                        Email = userInfo?.Email ?? "",
                        PhoneNumber = userInfo?.PhoneNumber ?? "",
                        ProfilePictureUrl = userInfo?.ProfilePictureUrl ?? "",
                        StaffType = s.StaffType.ToString(),
                        Department = s.Department,
                        Position = s.Position,
                        EmploymentStatus = s.EmploymentStatus.ToString(),
                        YearsOfExperience = s.YearsOfExperience
                    };
                }).ToList();

                var result = new PaginatedList<StaffSummaryDto>(
                    items,
                    totalCount,
                    request.PageNumber,
                    request.PageSize);

                return Result<PaginatedList<StaffSummaryDto>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving staff list");
                return Result<PaginatedList<StaffSummaryDto>>.Failure("Error retrieving staff list");
            }
        }
    }
}
