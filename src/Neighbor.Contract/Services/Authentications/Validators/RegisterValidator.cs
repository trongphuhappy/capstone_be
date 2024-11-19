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
            .NotEmpty().WithMessage("Phone number cannot be empty.")
            .Must(phone =>
                (phone.Length == 10 && phone.StartsWith("0") && phone.All(char.IsDigit)) ||
                (phone.Length == 9 && phone.All(char.IsDigit))
            )
            .WithMessage("Phone number must have either 10 digits starting with 0, or exactly 9 digits.");
    }
}