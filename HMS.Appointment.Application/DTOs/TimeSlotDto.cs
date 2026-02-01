namespace HMS.Appointment.Application.DTOs
{
    public class TimeSlotDto
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; }
        public int AvailableSlots { get; set; }
    }
}
