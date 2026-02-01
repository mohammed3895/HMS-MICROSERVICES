using HMS.Common.DTOs;
using MediatR;

namespace HMS.Staff.Application.Commands
{
    public class RejectStaffLeaveCommand : IRequest<Result<bool>>
    {
        public Guid LeaveId { get; set; }
        public string RejectionReason { get; set; } = string.Empty;
        public Guid RejectedBy { get; set; }
    }
}
