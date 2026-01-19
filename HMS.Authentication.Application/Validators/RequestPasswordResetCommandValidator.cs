using FluentValidation;
using HMS.Authentication.Application.Commands.Authentication;

namespace HMS.Authentication.Application.Validators
{
    public class RequestPasswordResetCommandValidator : AbstractValidator<RequestPasswordResetCommand>
    {
        public RequestPasswordResetCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");
        }
    }
}
