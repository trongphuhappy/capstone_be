using FluentValidation;
using Neighbor.Contract.Services.Orders;

namespace Neighbor.Contract.Services.Orders.Validators;

internal class AdminConfirmOrderValidator : AbstractValidator<Command.AdminConfirmOrderCommand>
{
    public AdminConfirmOrderValidator()
    {
        RuleFor(x => x.OrderId).NotNull().NotEmpty();
        RuleFor(x => x.IsApproved).NotNull();
    }
}
