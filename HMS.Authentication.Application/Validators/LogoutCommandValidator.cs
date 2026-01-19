using FluentValidation;
using HMS.Authentication.Application.Commands.Authentication;

namespace HMS.Authentication.Application.Validators
{
    public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
    {
        public LogoutCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");
        }
    }
}
