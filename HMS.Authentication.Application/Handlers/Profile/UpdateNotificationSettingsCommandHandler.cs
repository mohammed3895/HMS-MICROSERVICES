using HMS.Authentication.Application.Commands.Profile;
using HMS.Authentication.Domain.DTOs.Settings;
using HMS.Authentication.Domain.Entities;
using HMS.Authentication.Infrastructure.Data;
using HMS.Authentication.Infrastructure.Interfaces;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HMS.Authentication.Application.Handlers.Profile
{
    public class UpdateNotificationSettingsCommandHandler : IRequestHandler<UpdateNotificationSettingsCommand, Result<NotificationSettingsDto>>
    {
        private readonly AuthenticationDbContext _context;
        private readonly IAuditService _auditService;

        public UpdateNotificationSettingsCommandHandler(
            AuthenticationDbContext context,
            IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<Result<NotificationSettingsDto>> Handle(
            UpdateNotificationSettingsCommand request,
            CancellationToken cancellationToken)
        {
            var settings = await _context.UserSettings
                .FirstOrDefaultAsync(s => s.UserId == request.UserId, cancellationToken);

            if (settings == null)
            {
                settings = new UserSettings
                {
                    UserId = request.UserId,
                    NotificationSettings = new NotificationSettingsDto(),
                    PrivacySettings = new PrivacySettingsDto(),
                    Preferences = new PreferencesDto()
                };
                _context.UserSettings.Add(settings);
            }

            // Update only provided fields
            if (request.EmailNotifications.HasValue)
                settings.NotificationSettings.EmailNotifications = request.EmailNotifications.Value;

            if (request.SmsNotifications.HasValue)
                settings.NotificationSettings.SmsNotifications = request.SmsNotifications.Value;

            if (request.PushNotifications.HasValue)
                settings.NotificationSettings.PushNotifications = request.PushNotifications.Value;

            if (request.AppointmentReminders.HasValue)
                settings.NotificationSettings.AppointmentReminders = request.AppointmentReminders.Value;

            if (request.MedicationReminders.HasValue)
                settings.NotificationSettings.MedicationReminders = request.MedicationReminders.Value;

            if (request.NewsletterSubscription.HasValue)
                settings.NotificationSettings.NewsletterSubscription = request.NewsletterSubscription.Value;

            if (request.SecurityAlerts.HasValue)
                settings.NotificationSettings.SecurityAlerts = request.SecurityAlerts.Value;

            settings.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            await _auditService.LogAsync("NotificationSettingsUpdated", "UserSettings", settings.Id.ToString(),
                "Notification settings updated", request.UserId.ToString());

            return Result<NotificationSettingsDto>.Success(
                settings.NotificationSettings,
                "Notification settings updated successfully");
        }
    }
}
