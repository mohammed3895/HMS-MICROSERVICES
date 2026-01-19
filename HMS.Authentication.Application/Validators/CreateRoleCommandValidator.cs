using FluentValidation;
using HMS.Authentication.Application.Commands.Roles;

namespace HMS.Authentication.Application.Validators
{
    public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
    {
        public CreateRoleCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Role name is required")
                .MaximumLength(256).WithMessage("Role name cannot exceed 256 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
        }
    }
}
