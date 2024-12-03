using FluentValidation;
using Neighbor.Contract.Services.Orders;

namespace Neighbor.Contract.Services.Orders.Validators;

internal class UserConfirmOrderValidator : AbstractValidator<Command.UserConfirmOrderCommand>
{
    public UserConfirmOrderValidator()
    {
        RuleFor(x => x.OrderId).NotNull().NotEmpty();
        RuleFor(x => x.IsApproved).NotNull();
    }
}
