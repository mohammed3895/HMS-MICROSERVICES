using HMS.Appointment.Domain.Enums;

namespace HMS.Appointment.Domain.Entities
{
    public class AppointmentConflict
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public ConflictType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public ConflictSeverity Severity { get; set; }
        public bool IsResolved { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public string? Resolution { get; set; }
        public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
    }
}
