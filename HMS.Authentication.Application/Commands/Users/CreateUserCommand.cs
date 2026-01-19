using HMS.Authentication.Application.DTOs.Users;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Commands.Users
{
    public class CreateUserCommand : IRequest<Result<CreateUserResponse>>
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? NationalId { get; set; }
        public string? LicenseNumber { get; set; }
    }
}
