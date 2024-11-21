using FluentValidation;
using Neighbor.Contract.Services.Admins;

namespace Neighbor.Contract.Services.Admins.Validators;

internal class HandleUserValidator : AbstractValidator<Command.HandleUserCommand>
{
    public HandleUserValidator()
    {
        RuleFor(x => x.AccountId).NotNull().NotEmpty();
        RuleFor(x => x.IsDeleted).NotNull();
    }
}
