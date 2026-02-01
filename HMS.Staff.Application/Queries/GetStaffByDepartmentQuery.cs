using HMS.Common.DTOs;
using HMS.Staff.Application.DTOs;
using MediatR;

namespace HMS.Staff.Application.Queries
{
    public class GetStaffByDepartmentQuery : IRequest<Result<List<StaffSummaryDto>>>
    {
        public string Department { get; set; } = string.Empty;
    }
}
