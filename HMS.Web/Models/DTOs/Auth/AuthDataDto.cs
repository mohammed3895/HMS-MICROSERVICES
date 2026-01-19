namespace HMS.Web.Models.DTOs.Auth
{
    public class AuthDataDto
    {
        public Guid UserId { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool RequiresTwoFactor { get; set; }
        public bool RememberMe { get; set; }
        public bool RequiresEmailVerification { get; set; }
        public bool EmailConfirmationRequired { get; set; }
        public string? TwoFactorToken { get; set; }
        public string? Message { get; set; }
        public List<string>? Roles { get; set; }
    }
}
