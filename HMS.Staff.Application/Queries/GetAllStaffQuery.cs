using HMS.Common.DTOs;
using HMS.Staff.Application.DTOs;
using MediatR;

namespace HMS.Staff.Application.Queries
{
    public class GetAllStaffQuery : IRequest<Result<PaginatedList<StaffSummaryDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? StaffType { get; set; }
        public string? Department { get; set; }
        public string? EmploymentStatus { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }
    }
}
