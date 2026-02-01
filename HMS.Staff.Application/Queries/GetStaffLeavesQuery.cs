using HMS.Common.DTOs;
using HMS.Staff.Application.DTOs;
using MediatR;

namespace HMS.Staff.Application.Queries
{
    public class GetStaffLeavesQuery : IRequest<Result<List<StaffLeaveDto>>>
    {
        public Guid StaffId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Status { get; set; }
    }
}
