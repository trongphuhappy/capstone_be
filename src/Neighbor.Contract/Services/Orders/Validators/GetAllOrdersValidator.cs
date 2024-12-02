using FluentValidation;
using Neighbor.Contract.Services.Orders;

namespace Neighbor.Contract.Services.Orders.Validators;

internal class GetAllProductsInWishlistValidator : AbstractValidator<Query.GetAllOrdersQuery>
{
    public GetAllProductsInWishlistValidator()
    {
        // SortType and IsSortASC must either both exist or not exist
        RuleFor(x => x.FilterParams)
            .Must(filter => (filter.SortBy != null && filter.SortBy.SortType != null && filter.SortBy.IsSortASC != null) || (filter.SortBy == null))
            .WithMessage("SortType and IsSortASC must either both be provided or both be null.");
    }
}