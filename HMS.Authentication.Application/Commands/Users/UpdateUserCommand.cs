using MediatR;

namespace HMS.Authentication.Application.Commands.Users
{
    public class UpdateUserCommand : IRequest<Result<UpdateUserResponse>>
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}
