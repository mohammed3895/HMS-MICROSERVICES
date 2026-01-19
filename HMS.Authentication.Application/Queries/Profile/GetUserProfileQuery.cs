using HMS.Authentication.Application.DTOs.Profile;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Queries.Profile
{
    public class GetUserProfileQuery : IRequest<Result<UserProfileResponse>>
    {
        public Guid UserId { get; set; }
    }
}
