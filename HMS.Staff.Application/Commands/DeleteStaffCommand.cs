using HMS.Common.DTOs;
using MediatR;

namespace HMS.Staff.Application.Commands
{
    public class DeleteStaffCommand : IRequest<Result<bool>>
    {
        public Guid StaffId { get; set; }
        public Guid DeletedBy { get; set; }
    }
}
