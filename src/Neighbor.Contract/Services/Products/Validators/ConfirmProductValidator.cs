using FluentValidation;
using Neighbor.Contract.Services.Products;

namespace Neighbor.Contract.Services.Products.Validators;

internal class ConfirmProductValidator : AbstractValidator<Command.ConfirmProductCommand>
{
    public ConfirmProductValidator()
    {
        RuleFor(x => x.ProductId).NotNull().NotEmpty();
        RuleFor(x => x.ConfirmStatus).NotEmpty();
    }
}
