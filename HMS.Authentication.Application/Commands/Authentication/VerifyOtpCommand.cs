using MediatR;

namespace HMS.Authentication.Application.Commands.Authentication
{
    public class VerifyOtpCommand : IRequest<Result<VerifyOtpResponse>>
    {
        public string Email { get; set; }
        public string OtpCode { get; set; }
    }
}
