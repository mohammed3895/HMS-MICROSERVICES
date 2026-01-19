using HMS.Authentication.Application.DTOs.Authentication;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Commands.Authentication
{
    public class RefreshTokenCommand : IRequest<Result<LoginResponse>>
    {
        public Guid UserId { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public string? DeviceId { get; set; }
    }
}
