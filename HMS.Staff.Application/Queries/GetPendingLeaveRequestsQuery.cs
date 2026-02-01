using HMS.Common.DTOs;
using HMS.Staff.Application.DTOs;
using MediatR;

namespace HMS.Staff.Application.Queries
{
    public class GetPendingLeaveRequestsQuery : IRequest<Result<List<StaffLeaveDto>>>
    {
        public Guid? StaffId { get; set; }
    }
}
