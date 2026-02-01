namespace HMS.Appointment.Domain.Entities
{
    public class ScheduleException
    {
        public Guid Id { get; set; }
        public Guid DoctorScheduleId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan? NewStartTime { get; set; }
        public TimeSpan? NewEndTime { get; set; }
        public bool IsClosed { get; set; }
        public string? Reason { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual DoctorSchedule Schedule { get; set; } = null!;
    }
}
