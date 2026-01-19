namespace HMS.Authentication.Domain.Entities
{
    public class SecurityEvent
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string EventType { get; set; } = string.Empty; // Suspicious login, Multiple failed attempts, etc.
        public string Severity { get; set; } = "Low"; // Low, Medium, High, Critical
        public string Description { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
        public string? DeviceId { get; set; }
        public string? UserAgent { get; set; }
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
        public bool IsResolved { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public string? Resolution { get; set; }

        public virtual ApplicationUser? User { get; set; }
    }
}
