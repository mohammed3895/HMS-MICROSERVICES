namespace HMS.Authentication.Application.DTOs.Settings
{
    public class PreferencesDto
    {
        public string Language { get; set; } = "en";
        public string TimeZone { get; set; } = "UTC";
        public string DateFormat { get; set; } = "MM/dd/yyyy";
        public string TimeFormat { get; set; } = "12h";
        public string Theme { get; set; } = "light";
    }
}
