using FluentValidation;
using HMS.Authentication.Application.Commands.Authentication;

namespace HMS.Authentication.Application.Validators
{
    public class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
    {
        public ConfirmEmailCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.OtpCode)
                .NotEmpty().WithMessage("OTP code is required")
                .Length(6).WithMessage("OTP code must be 6 digits")
                .Matches(@"^\d{6}$").WithMessage("OTP code must contain only digits");
        }
    }
}
