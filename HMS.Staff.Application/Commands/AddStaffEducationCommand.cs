using HMS.Common.DTOs;
using MediatR;

namespace HMS.Staff.Application.Commands
{
    public class AddStaffEducationCommand : IRequest<Result<Guid>>
    {
        public Guid StaffId { get; set; }
        public string Degree { get; set; } = string.Empty;
        public string Institution { get; set; } = string.Empty;
        public string? FieldOfStudy { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public string? Country { get; set; }
    }
}
