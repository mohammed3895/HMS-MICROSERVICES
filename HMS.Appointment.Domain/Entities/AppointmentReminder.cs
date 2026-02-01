using HMS.Appointment.Domain.Enums;

namespace HMS.Appointment.Domain.Entities
{
    public class AppointmentReminder
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public ReminderType Type { get; set; } // SMS, Email, Push, WhatsApp
        public ReminderTiming Timing { get; set; } // 24Hours, 2Hours, 30Minutes
        public bool IsSent { get; set; }
        public DateTime? SentAt { get; set; }
        public bool IsDelivered { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime ScheduledFor { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual Appointment Appointment { get; set; } = null!;
    }
}
