using FluentValidation;
using HMS.Authentication.Application.Commands.Authentication;

namespace HMS.Authentication.Application.Validators
{
    public class ResendOtpCommandValidator : AbstractValidator<ResendOtpCommand>
    {
        public ResendOtpCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Purpose)
                .NotEmpty().WithMessage("Purpose is required")
                .Must(p => new[] { "registration", "login", "password-reset" }.Contains(p))
                .WithMessage("Invalid purpose");
        }
    }
}
