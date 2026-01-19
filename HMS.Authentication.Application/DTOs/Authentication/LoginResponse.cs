namespace HMS.Authentication.Application.DTOs.Authentication
{
    public class LoginResponse
    {
        public Guid UserId { get; set; }
        public string? Email { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool RequiresTwoFactor { get; set; }
        public bool RequiresEmailVerification { get; set; }
        public bool RequiresOtp { get; set; }
        public string? TwoFactorToken { get; set; }
        public string? Message { get; set; }
        public UserInfoDto? UserInfo { get; set; }
    }
}
