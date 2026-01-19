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
    public class UpdatePrivacySettingsCommandHandler : IRequestHandler<UpdatePrivacySettingsCommand, Result<PrivacySettingsDto>>
    {
        private readonly AuthenticationDbContext _context;
        private readonly IAuditService _auditService;

        public UpdatePrivacySettingsCommandHandler(
            AuthenticationDbContext context,
            IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<Result<PrivacySettingsDto>> Handle(
            UpdatePrivacySettingsCommand request,
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
            if (request.ProfileVisibility.HasValue)
                settings.PrivacySettings.ProfileVisibility = request.ProfileVisibility.Value;

            if (request.ShowEmail.HasValue)
                settings.PrivacySettings.ShowEmail = request.ShowEmail.Value;

            if (request.ShowPhoneNumber.HasValue)
                settings.PrivacySettings.ShowPhoneNumber = request.ShowPhoneNumber.Value;

            if (request.AllowDataSharing.HasValue)
                settings.PrivacySettings.AllowDataSharing = request.AllowDataSharing.Value;

            settings.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            await _auditService.LogAsync("PrivacySettingsUpdated", "UserSettings", settings.Id.ToString(),
                "Privacy settings updated", request.UserId.ToString());

            return Result<PrivacySettingsDto>.Success(
                settings.PrivacySettings,
                "Privacy settings updated successfully");
        }
    }
}
