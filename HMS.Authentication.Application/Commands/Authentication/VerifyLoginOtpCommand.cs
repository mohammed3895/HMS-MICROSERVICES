using HMS.Authentication.Application.DTOs.Authentication;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Commands.Authentication
{
    public class VerifyLoginOtpCommand : IRequest<Result<LoginResponse>>
    {
        public Guid UserId { get; set; }
        public string OtpCode { get; set; } = string.Empty;
        public string? TwoFactorToken { get; set; }
        public string? DeviceId { get; set; }
        public bool TrustDevice { get; set; }
    }
}
