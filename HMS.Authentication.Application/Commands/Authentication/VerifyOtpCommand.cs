using HMS.Authentication.Application.DTOs.Authentication;
using HMS.Common.DTOs;
using MediatR;

namespace HMS.Authentication.Application.Commands.Authentication
{
    public class VerifyOtpCommand : IRequest<Result<VerifyOtpResponse>>
    {
        public Guid UserId { get; set; }
        public string OtpCode { get; set; }
    }
}
