using HMS.Authentication.Domain.Enums;

namespace HMS.Authentication.Domain.Entities
{
    public class AuditLogFilter
    {
        public Guid? UserId { get; set; }
        public string? Username { get; set; }
        public AuditAction? Action { get; set; }
        public string? EntityName { get; set; }
        public string? EntityId { get; set; }
        public string? IpAddress { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsSuccessful { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SortBy { get; set; } = "Timestamp";
        public bool SortDescending { get; set; } = true;
    }
}
