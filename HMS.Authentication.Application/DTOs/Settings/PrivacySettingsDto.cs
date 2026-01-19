namespace HMS.Authentication.Application.DTOs.Settings
{
    public class PrivacySettingsDto
    {
        public bool ProfileVisibility { get; set; } = true;
        public bool ShowEmail { get; set; } = false;
        public bool ShowPhoneNumber { get; set; } = false;
        public bool AllowDataSharing { get; set; } = false;
        public bool TwoFactorEnabled { get; set; } = false;
    }
}
