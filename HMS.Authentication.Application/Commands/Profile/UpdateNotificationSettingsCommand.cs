using HMS.Authentication.Domain.DTOs.Settings;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Commands.Profile
{
    public class UpdateNotificationSettingsCommand : IRequest<Result<NotificationSettingsDto>>
    {
        public Guid UserId { get; set; }
        public bool? EmailNotifications { get; set; }
        public bool? SmsNotifications { get; set; }
        public bool? PushNotifications { get; set; }
        public bool? AppointmentReminders { get; set; }
        public bool? MedicationReminders { get; set; }
        public bool? NewsletterSubscription { get; set; }
        public bool? SecurityAlerts { get; set; }
    }
}
