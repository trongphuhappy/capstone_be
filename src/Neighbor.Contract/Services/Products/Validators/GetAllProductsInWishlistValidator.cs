using FluentValidation;
using Neighbor.Contract.Services.Products;

namespace Neighbor.Contract.Services.Products.Validators;

internal class GetAllProductsInWishlistValidator : AbstractValidator<Query.GetAllProductsInWishlistQuery>
{
    public GetAllProductsInWishlistValidator()
    {
        RuleFor(x => x.AccountId).NotNull().NotEmpty();
    }
}