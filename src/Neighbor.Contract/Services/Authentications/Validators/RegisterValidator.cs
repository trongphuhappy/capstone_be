using FluentValidation;

namespace Neighbor.Contract.Services.Authentications.Validators;

internal class RegisterValidator : AbstractValidator<Command.RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.FirstName)
           .NotEmpty().WithMessage("First name is required.")
           .MinimumLength(3).WithMessage("First name must be at least 3 characters long.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MinimumLength(3).WithMessage("Last name must be at least 3 characters long.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
            .MaximumLength(20).WithMessage("Password cannot exceed 20 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required");
    }
}