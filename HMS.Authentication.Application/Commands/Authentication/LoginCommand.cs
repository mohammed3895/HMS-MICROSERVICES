using MediatR;

namespace HMS.Authentication.Application.Commands.Authentication
{
    public class LoginCommand : IRequest<Result<LoginResponse>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string? DeviceId { get; set; }
    }

}
