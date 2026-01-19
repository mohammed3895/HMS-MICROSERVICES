using FluentValidation;
using HMS.Authentication.Application.Commands.Authentication;

namespace HMS.Authentication.Application.Validators
{
    public class VerifyLoginOtpCommandValidator : AbstractValidator<VerifyLoginOtpCommand>
    {
        public VerifyLoginOtpCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");

            RuleFor(x => x.OtpCode)
                .NotEmpty().WithMessage("OTP code is required")
                .Length(6).WithMessage("OTP code must be 6 digits")
                .Matches(@"^\d{6}$").WithMessage("OTP code must contain only digits");
        }
    }

}
