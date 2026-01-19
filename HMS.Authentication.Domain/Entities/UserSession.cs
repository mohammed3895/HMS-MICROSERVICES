namespace HMS.Authentication.Domain.Entities
{
    public class UserSession
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string SessionToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string DeviceId { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
        public DateTime? LastActivityAt { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsRevoked { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string? RevokeReason { get; set; }

        public virtual ApplicationUser User { get; set; } = null!;
    }
}
