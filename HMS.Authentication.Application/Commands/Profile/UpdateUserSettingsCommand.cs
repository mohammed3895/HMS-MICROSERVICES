using HMS.Authentication.Domain.DTOs.Settings;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Commands.Profile
{
    public class UpdateUserSettingsCommand : IRequest<Result<UserSettingsResponse>>
    {
        public Guid UserId { get; set; }
        public NotificationSettingsDto? NotificationSettings { get; set; }
        public PrivacySettingsDto? PrivacySettings { get; set; }
        public PreferencesDto? Preferences { get; set; }
    }
}
