namespace HMS.Authentication.Domain.Entities
{
    public class OtpVerification
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
        public string Purpose { get; set; } = "login"; // login, registration, password-reset
        public string? IpAddress { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
        public int Attempts { get; set; }
        public bool IsVerified { get; set; }
        public DateTime? VerifiedAt { get; set; }
    }
}
