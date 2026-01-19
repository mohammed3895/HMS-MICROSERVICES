namespace HMS.Authentication.Domain.DTOs.Settings
{
    public class NotificationSettingsDto
    {
        public bool EmailNotifications { get; set; } = true;
        public bool SmsNotifications { get; set; } = false;
        public bool PushNotifications { get; set; } = true;
        public bool AppointmentReminders { get; set; } = true;
        public bool MedicationReminders { get; set; } = true;
        public bool NewsletterSubscription { get; set; } = false;
        public bool SecurityAlerts { get; set; } = true;
    }
}
