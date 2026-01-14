using HMS.Authentication.Domain.Enums;

namespace HMS.Authentication.Domain.Entities
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string? Username { get; set; }
        public AuditAction Action { get; set; }
        public string EntityName { get; set; }
        public string? EntityId { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? Changes { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime Timestamp { get; set; }
        public string? AdditionalInfo { get; set; }
        public bool IsSuccessful { get; set; } = true;
        public string? ErrorMessage { get; set; }
    }
}