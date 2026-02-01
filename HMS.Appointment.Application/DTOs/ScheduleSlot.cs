namespace HMS.Appointment.Application.DTOs
{
    public class ScheduleSlot
    {
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int SlotDurationMinutes { get; set; } = 30;
    }
}
