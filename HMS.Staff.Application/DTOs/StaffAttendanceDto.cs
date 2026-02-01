namespace HMS.Staff.Application.DTOs
{
    public class StaffAttendanceDto
    {
        public Guid Id { get; set; }
        public Guid StaffId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
