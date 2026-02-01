namespace HMS.Appointment.Domain.Entities
{
    public class AppointmentHistory
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public string Action { get; set; } = string.Empty; // Created, Rescheduled, Cancelled, CheckedIn, etc.
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? Reason { get; set; }
        public Guid PerformedBy { get; set; }
        public string PerformedByName { get; set; } = string.Empty;
        public DateTime PerformedAt { get; set; } = DateTime.UtcNow;

        public virtual Appointment Appointment { get; set; } = null!;
    }
}
