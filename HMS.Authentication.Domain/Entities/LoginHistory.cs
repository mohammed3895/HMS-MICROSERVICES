namespace HMS.Authentication.Domain.Entities
{
    public class LoginHistory
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime LoginTime { get; set; } = DateTime.UtcNow;
        public DateTime? LogoutTime { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public string? DeviceId { get; set; }
        public bool IsSuccess { get; set; }
        public string? FailureReason { get; set; }
        public string LoginMethod { get; set; } = "Password"; // Password, OTP, WebAuthn, 2FA
        public string? Location { get; set; } // City, Country from IP
        public bool WasOtpUsed { get; set; }
        public bool Was2FAUsed { get; set; }

        public virtual ApplicationUser User { get; set; } = null!;
    }
}
