using HMS.Common.DTOs;
using MediatR;

namespace HMS.Staff.Application.Commands
{
    public class ApplyStaffLeaveCommand : IRequest<Result<Guid>>
    {
        public Guid StaffId { get; set; }
        public string LeaveType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Reason { get; set; }
    }
}
