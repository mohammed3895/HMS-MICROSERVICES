namespace HMS.Appointment.Domain.Entities
{
    public class TimeSlot
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int MaxCapacity { get; set; } = 1;
        public int BookedCount { get; set; }
        public bool IsAvailable { get; set; } = true;
        public bool IsBlocked { get; set; }
        public string? BlockReason { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
