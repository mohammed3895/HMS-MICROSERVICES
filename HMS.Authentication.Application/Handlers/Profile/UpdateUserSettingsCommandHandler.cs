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
    public class UpdateUserSettingsCommandHandler : IRequestHandler<UpdateUserSettingsCommand, Result<UserSettingsResponse>>
    {
        private readonly AuthenticationDbContext _context;
        private readonly IAuditService _auditService;

        public UpdateUserSettingsCommandHandler(
            AuthenticationDbContext context,
            IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<Result<UserSettingsResponse>> Handle(
            UpdateUserSettingsCommand request,
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

            // Update notification settings
            if (request.NotificationSettings != null)
            {
                settings.NotificationSettings = request.NotificationSettings;
            }

            // Update privacy settings
            if (request.PrivacySettings != null)
            {
                settings.PrivacySettings = request.PrivacySettings;
            }

            // Update preferences
            if (request.Preferences != null)
            {
                settings.Preferences = request.Preferences;
            }

            settings.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            await _auditService.LogAsync("UserSettingsUpdated", "UserSettings", settings.Id.ToString(),
                "User settings updated", request.UserId.ToString());

            var response = new UserSettingsResponse
            {
                UserId = settings.UserId,
                NotificationSettings = settings.NotificationSettings,
                PrivacySettings = settings.PrivacySettings,
                Preferences = settings.Preferences
            };

            return Result<UserSettingsResponse>.Success(response, "Settings updated successfully");
        }
    }
}
