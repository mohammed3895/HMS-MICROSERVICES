using HMS.Authentication.Domain.DTOs.Settings;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Queries.Profile
{
    public class GetUserSettingsQuery : IRequest<Result<UserSettingsResponse>>
    {
        public Guid UserId { get; set; }
    }
}
