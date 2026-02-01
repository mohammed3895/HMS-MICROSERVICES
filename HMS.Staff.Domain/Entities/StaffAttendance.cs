using HMS.Staff.Domain.Enums;

namespace HMS.Staff.Domain.Entities
{
    public class StaffAttendance
    {
        public Guid Id { get; set; }
        public Guid StaffId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        public AttendanceStatus Status { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }

        public Staff Staff { get; set; } = null!;
    }
}
