using HMS.Common.DTOs;
using HMS.Staff.Application.DTOs;
using MediatR;

namespace HMS.Staff.Application.Queries
{
    public class GetStaffByTypeQuery : IRequest<Result<List<StaffSummaryDto>>>
    {
        public string StaffType { get; set; } = string.Empty;
    }
}
