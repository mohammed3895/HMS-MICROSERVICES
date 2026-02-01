using HMS.Common.DTOs;
using HMS.Staff.Application.DTOs;
using MediatR;

namespace HMS.Staff.Application.Queries
{
    public class SearchStaffQuery : IRequest<Result<List<StaffSummaryDto>>>
    {
        public string? SearchTerm { get; set; }
        public string? StaffType { get; set; }
        public string? Department { get; set; }
        public string? Specialization { get; set; }
    }
}
