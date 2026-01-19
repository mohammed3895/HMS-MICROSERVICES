namespace HMS.Authentication.Infrastructure.Configuration
{
    public class JwtSettings
    {
        public string Secret { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpiryInHours { get; set; } = 1;
        public int RefreshTokenExpiryInDays { get; set; } = 7;
    }
}
