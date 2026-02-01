using HMS.Common.DTOs;
using HMS.Staff.Application.DTOs;
using MediatR;

namespace HMS.Staff.Application.Queries
{
    public class GetStaffByIdQuery : IRequest<Result<StaffDetailsDto>>
    {
        public Guid StaffId { get; set; }
    }
}
