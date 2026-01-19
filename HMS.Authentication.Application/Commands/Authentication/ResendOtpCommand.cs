using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Commands.Authentication
{
    public class ResendOtpCommand : IRequest<Result<object>>
    {
        public string Email { get; set; } = string.Empty;
        public string Purpose { get; set; } = "registration"; // registration or login
    }
}
