using FluentValidation;
using Neighbor.Contract.Services.Orders;

namespace Neighbor.Contract.Services.Orders.Validators;

internal class CreateOrderBankingValidator : AbstractValidator<Command.CreateOrderBankingCommand>
{
    public CreateOrderBankingValidator()
    {
        RuleFor(x => x.AccountId).NotNull().NotEmpty();
        RuleFor(x => x.ProductId).NotNull().NotEmpty();
        RuleFor(x => x.RentTime)
            .NotNull().WithMessage("Rent Time must not be null.")
            .NotEmpty().WithMessage("Rent Time must not be empty.")
            .GreaterThan(DateTime.UtcNow).WithMessage("Rent Time must be in the future.");

        RuleFor(x => x.ReturnTime)
            .NotNull().WithMessage("Return Time must not be null.")
            .NotEmpty().WithMessage("Return Time must not be empty.")
            .GreaterThan(DateTime.UtcNow).WithMessage("Return Time must be in the future.");

        RuleFor(x => x)
            .Must(x => x.RentTime < x.ReturnTime)
            .WithMessage("Rent Time must be earlier than Return Time.");
    }
}
