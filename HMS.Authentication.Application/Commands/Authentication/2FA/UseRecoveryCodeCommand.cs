using HMS.Authentication.Application.DTOs.Authentication;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Commands.Authentication
{
    public class UseRecoveryCodeCommand : IRequest<Result<LoginResponse>>
    {
        public Guid UserId { get; set; }
        public string RecoveryCode { get; set; } = string.Empty;
        public string TwoFactorToken { get; set; } = string.Empty;
        public string? DeviceId { get; set; }
    }
}
