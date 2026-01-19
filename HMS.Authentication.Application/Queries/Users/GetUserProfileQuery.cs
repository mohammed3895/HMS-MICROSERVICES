using HMS.Authentication.Application.DTOs.Users;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Queries.Users
{
    public class GetUserProfileQuery : IRequest<Result<UserProfileDto>>
    {
        public Guid UserId { get; set; }
    }
}
