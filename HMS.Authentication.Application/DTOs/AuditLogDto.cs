using HMS.Authentication.Domain.Enums;

namespace HMS.Authentication.Application.DTOs
{
    public class AuditLogDto
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
        public DateTime Timestamp { get; set; }
        public bool IsSuccessful { get; set; }
    }
}
