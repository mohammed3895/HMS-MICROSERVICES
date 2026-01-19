using HMS.Authentication.Application.Queries.Profile;
using HMS.Authentication.Domain.DTOs.Settings;
using HMS.Authentication.Infrastructure.Data;
using HMS.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HMS.Authentication.Application.Handlers.Profile
{
    public class GetUserSettingsQueryHandler : IRequestHandler<GetUserSettingsQuery, Result<UserSettingsResponse>>
    {
        private readonly AuthenticationDbContext _context;

        public GetUserSettingsQueryHandler(AuthenticationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<UserSettingsResponse>> Handle(
            GetUserSettingsQuery request,
            CancellationToken cancellationToken)
        {
            var settings = await _context.UserSettings
                .FirstOrDefaultAsync(s => s.UserId == request.UserId, cancellationToken);

            if (settings == null)
            {
                // Return default settings if none exist
                return Result<UserSettingsResponse>.Success(new UserSettingsResponse
                {
                    UserId = request.UserId,
                    NotificationSettings = new NotificationSettingsDto(),
                    PrivacySettings = new PrivacySettingsDto(),
                    Preferences = new PreferencesDto()
                });
            }

            var response = new UserSettingsResponse
            {
                UserId = settings.UserId,
                NotificationSettings = settings.NotificationSettings ?? new NotificationSettingsDto(),
                PrivacySettings = settings.PrivacySettings ?? new PrivacySettingsDto(),
                Preferences = settings.Preferences ?? new PreferencesDto()
            };

            return Result<UserSettingsResponse>.Success(response);
        }
    }
}
