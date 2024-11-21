using FluentValidation;
using Neighbor.Contract.Services.Products;

namespace Neighbor.Contract.Services.Products.Validators;

internal class HandleUserValidator : AbstractValidator<Command.ConfirmProductCommand>
{
    public HandleUserValidator()
    {
        RuleFor(x => x.ProductId).NotNull().NotEmpty();
        RuleFor(x => x.ConfirmStatus).NotEmpty();
    }
}
