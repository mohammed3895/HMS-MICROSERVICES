using HMS.Common.DTOs;
using HMS.Staff.Application.DTOs;
using MediatR;

namespace HMS.Staff.Application.Commands
{
    public class CreateStaffCommand : IRequest<Result<StaffWithUserInfoDto>>
    {
        public CreateStaffWithAuthDto Data { get; set; } = null!;
        public Guid CreatedBy { get; set; }
    }
}
