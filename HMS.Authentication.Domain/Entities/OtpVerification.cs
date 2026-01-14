using HMS.Authentication.Domain.Enums;

namespace HMS.Authentication.Domain.Entities
{
    public class OtpVerification
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string OtpCode { get; set; }
        public OtpType Type { get; set; } // Login, Registration, PasswordReset, TwoFactor
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public DateTime? UsedAt { get; set; }
    }
}
