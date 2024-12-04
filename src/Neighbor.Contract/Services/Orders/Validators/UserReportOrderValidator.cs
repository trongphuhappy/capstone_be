using FluentValidation;
using Neighbor.Contract.Services.Orders;

namespace Neighbor.Contract.Services.Orders.Validators;

internal class UserReportOrderValidator : AbstractValidator<Command.UserReportOrderCommand>
{
    public UserReportOrderValidator()
    {
        RuleFor(x => x.OrderId).NotNull();
        RuleFor(x => x.UserReport).NotNull().NotEmpty();
    }
}
