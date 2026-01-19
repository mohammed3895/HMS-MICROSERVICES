using FluentValidation;
using HMS.Authentication.Application.Commands.Authentication;

namespace HMS.Authentication.Application.Validators
{
    public class Verify2FASetupCommandValidator : AbstractValidator<Verify2FASetupCommand>
    {
        public Verify2FASetupCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Verification code is required")
                .Length(6).WithMessage("Verification code must be 6 digits")
                .Matches(@"^\d{6}$").WithMessage("Verification code must contain only digits");
        }
    }
}
