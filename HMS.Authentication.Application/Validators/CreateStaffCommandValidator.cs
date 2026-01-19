using FluentValidation;
using HMS.Authentication.Application.Commands.Admin;

namespace HMS.Authentication.Application.Validators
{
    public class CreateStaffCommandValidator : AbstractValidator<CreateStaffCommand>
    {
        public CreateStaffCommandValidator()
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
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required")
                .LessThan(DateTime.UtcNow.AddYears(-18)).WithMessage("Staff member must be at least 18 years old")
                .GreaterThan(DateTime.UtcNow.AddYears(-100)).WithMessage("Invalid date of birth");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role is required")
                .Must(role => new[] { "Doctor", "Nurse", "Pharmacist", "LabTechnician", "Receptionist", "Admin", "Manager" }.Contains(role))
                .WithMessage("Invalid role. Must be one of: Doctor, Nurse, Pharmacist, LabTechnician, Receptionist, Admin, Manager");

            RuleFor(x => x.Department)
                .MaximumLength(100).WithMessage("Department cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.Department));

            RuleFor(x => x.EmployeeId)
                .MaximumLength(50).WithMessage("Employee ID cannot exceed 50 characters")
                .When(x => !string.IsNullOrEmpty(x.EmployeeId));

            // Role-specific validations
            RuleFor(x => x.LicenseNumber)
                .NotEmpty().WithMessage("License number is required for medical professionals")
                .When(x => x.Role == "Doctor" || x.Role == "Nurse" || x.Role == "Pharmacist");

            RuleFor(x => x.LicenseExpiryDate)
                .NotEmpty().WithMessage("License expiry date is required for medical professionals")
                .GreaterThan(DateTime.UtcNow).WithMessage("License must not be expired")
                .When(x => x.Role == "Doctor" || x.Role == "Nurse" || x.Role == "Pharmacist");

            RuleFor(x => x.Specialization)
                .NotEmpty().WithMessage("Specialization is required for doctors")
                .When(x => x.Role == "Doctor");

            RuleFor(x => x.CreatedBy)
                .NotEmpty().WithMessage("Created by admin ID is required");
        }
    }
}
