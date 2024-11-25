using FluentValidation;
using Neighbor.Contract.Services.Products;

namespace Neighbor.Contract.Services.Products.Validators;

internal class AddToWishlistValidator : AbstractValidator<Command.AddToWishlistCommand>
{
    public AddToWishlistValidator()
    {
        RuleFor(x => x.AccountId).NotNull().NotEmpty();
        RuleFor(x => x.ProductId).NotNull().NotEmpty();
    }
}
