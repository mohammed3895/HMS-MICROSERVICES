namespace HMS.Authentication.Infrastructure.Configuration
{
    public class SecuritySettings
    {
        public int MaxFailedAccessAttempts { get; set; } = 5;
        public int LockoutTimeInMinutes { get; set; } = 15;
        public int PasswordResetTokenExpiryInHours { get; set; } = 1;
        public int OtpExpiryInMinutes { get; set; } = 10;
        public bool RequireConfirmedEmail { get; set; } = true;
        public bool RequireUniqueEmail { get; set; } = true;
    }
}
