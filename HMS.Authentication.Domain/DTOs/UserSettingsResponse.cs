namespace HMS.Authentication.Domain.DTOs.Settings
{
    public class UserSettingsResponse
    {
        public Guid UserId { get; set; }
        public NotificationSettingsDto NotificationSettings { get; set; } = new();
        public PrivacySettingsDto PrivacySettings { get; set; } = new();
        public PreferencesDto Preferences { get; set; } = new();
    }
}
