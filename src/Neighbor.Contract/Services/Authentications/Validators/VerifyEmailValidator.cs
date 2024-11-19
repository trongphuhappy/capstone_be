using FluentValidation;

namespace Neighbor.Contract.Services.Authentications.Validators;
internal class VerifyEmailValidator : AbstractValidator<Command.VerifyEmailCommand>
{
    public VerifyEmailValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
    }
}
