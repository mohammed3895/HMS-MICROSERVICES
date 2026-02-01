namespace HMS.Appointment.Domain.Entities
{
    public class DoctorSchedule
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int SlotDurationMinutes { get; set; } = 30;
        public int MaxPatientsPerSlot { get; set; } = 1;
        public bool IsActive { get; set; } = true;
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<ScheduleException> Exceptions { get; set; } = new List<ScheduleException>();
    }
}
