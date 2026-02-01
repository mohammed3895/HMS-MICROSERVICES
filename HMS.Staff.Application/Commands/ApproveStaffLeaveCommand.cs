using HMS.Common.DTOs;
using MediatR;

namespace HMS.Staff.Application.Commands
{
    public class ApproveStaffLeaveCommand : IRequest<Result<bool>>
    {
        public Guid LeaveId { get; set; }
        public Guid ApprovedBy { get; set; }
    }
}
