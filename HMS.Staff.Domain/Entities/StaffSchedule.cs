namespace HMS.Staff.Domain.Entities
{
    public class StaffSchedule
    {
        public Guid Id { get; set; }
        public Guid StaffId { get; set; }
        public DateTime ScheduleDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public ScheduleType Type { get; set; } // Regular, OnCall, Overtime
        public string? Notes { get; set; }
        public bool IsActive { get; set; }

        public Staff Staff { get; set; }
    }
}
