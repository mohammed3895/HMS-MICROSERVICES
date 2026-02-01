using HMS.Appointment.Domain.Enums;

namespace HMS.Appointment.Domain.Entities
{
    public class WaitlistEntry
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string PatientPhone { get; set; } = string.Empty;
        public string PatientEmail { get; set; } = string.Empty;
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public DateTime PreferredDate { get; set; }
        public TimeSpan? PreferredStartTime { get; set; }
        public TimeSpan? PreferredEndTime { get; set; }
        public WaitlistPriority Priority { get; set; }
        public WaitlistStatus Status { get; set; } // Active, Converted, Expired, Cancelled
        public DateTime? ConvertedToAppointmentAt { get; set; }
        public Guid? AppointmentId { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }

        public virtual Appointment? Appointment { get; set; }
    }
}
