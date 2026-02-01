using HMS.Common.DTOs;
using MediatR;

namespace HMS.Staff.Application.Commands
{
    public class RecordAttendanceCommand : IRequest<Result<Guid>>
    {
        public Guid StaffId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public Guid CreatedBy { get; set; }
    }
}
