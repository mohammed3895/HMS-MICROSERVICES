using HMS.Common.DTOs;
using MediatR;

namespace HMS.Staff.Application.Commands
{
    public class UpdateStaffStatusCommand : IRequest<Result<bool>>
    {
        public Guid StaffId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public Guid UpdatedBy { get; set; }
    }
}
