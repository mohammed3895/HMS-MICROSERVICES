using HMS.Common.DTOs;
using HMS.Staff.Application.DTOs;
using MediatR;

namespace HMS.Staff.Application.Queries
{
    public class GetStaffDashboardQuery : IRequest<Result<StaffDashboardDto>>
    {
        public Guid StaffId { get; set; }
    }
}
