namespace HMS.Authentication.Domain.Entities
{
    public class UserDevice
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string DeviceId { get; set; } = string.Empty;
        public string DeviceName { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public bool IsTrusted { get; set; }
        public DateTime LastUsedAt { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ApplicationUser User { get; set; } = null!;
    }
}
