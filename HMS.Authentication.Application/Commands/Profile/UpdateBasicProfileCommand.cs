using HMS.Authentication.Application.DTOs.Profile;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Commands.Profile
{
    public class UpdateBasicProfileCommand : IRequest<Result<UserProfileResponse>>
    {
        public Guid UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? NationalId { get; set; }
    }
}
