using HMS.Authentication.Application.DTOs.Authentication;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Commands.Authentication
{
    public class LoginCommand : IRequest<Result<LoginResponse>>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? DeviceId { get; set; }
        public bool RememberMe { get; set; }
    }

}
