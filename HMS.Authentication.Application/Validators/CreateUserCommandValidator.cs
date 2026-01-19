using FluentValidation;
using HMS.Authentication.Application.Commands.Users;

namespace HMS.Authentication.Application.Validators
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email format is invalid");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(100).WithMessage("First name cannot exceed 100 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters")
                .Matches(@"[A-Z]").WithMessage("Password must contain an uppercase letter")
                .Matches(@"[a-z]").WithMessage("Password must contain a lowercase letter")
                .Matches(@"[0-9]").WithMessage("Password must contain a number")
                .Matches(@"[!@#$%^&*]").WithMessage("Password must contain a special character");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required")
                .LessThan(DateTime.UtcNow.AddYears(-18)).WithMessage("User must be at least 18 years old");
        }
    }
}
