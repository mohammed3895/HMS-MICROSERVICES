using FluentValidation;
using HMS.Authentication.Application.Commands.Authentication;

namespace HMS.Authentication.Application.Validators
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(256).WithMessage("Email cannot exceed 256 characters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters")
                .MaximumLength(128).WithMessage("Password cannot exceed 128 characters")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one number")
                .Matches(@"[!@#$%^&*(),.?\"":{}|<>]").WithMessage("Password must contain at least one special character");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Password confirmation is required")
                .Equal(x => x.Password).WithMessage("Passwords do not match");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(100).WithMessage("First name cannot exceed 100 characters")
                .Matches(@"^[a-zA-Z\s\-']+$").WithMessage("First name can only contain letters, spaces, hyphens, and apostrophes");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters")
                .Matches(@"^[a-zA-Z\s\-']+$").WithMessage("Last name can only contain letters, spaces, hyphens, and apostrophes");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format. Use E.164 format (e.g., +1234567890)");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required")
                .LessThan(DateTime.UtcNow.AddYears(-13)).WithMessage("You must be at least 13 years old to register")
                .GreaterThan(DateTime.UtcNow.AddYears(-120)).WithMessage("Invalid date of birth");

            RuleFor(x => x.NationalId)
                .MaximumLength(50).WithMessage("National ID cannot exceed 50 characters")
                .When(x => !string.IsNullOrEmpty(x.NationalId));

            RuleFor(x => x.Address)
                .MaximumLength(500).WithMessage("Address cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Address));

            RuleFor(x => x.City)
                .MaximumLength(100).WithMessage("City cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.City));

            RuleFor(x => x.State)
                .MaximumLength(100).WithMessage("State cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.State));

            RuleFor(x => x.Country)
                .MaximumLength(100).WithMessage("Country cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.Country));

            RuleFor(x => x.PostalCode)
                .MaximumLength(20).WithMessage("Postal code cannot exceed 20 characters")
                .When(x => !string.IsNullOrEmpty(x.PostalCode));
        }
    }
}
