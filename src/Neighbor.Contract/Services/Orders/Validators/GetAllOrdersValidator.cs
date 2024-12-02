using FluentValidation;
using Neighbor.Contract.Services.Orders;

namespace Neighbor.Contract.Services.Orders.Validators;

internal class GetAllProductsInWishlistValidator : AbstractValidator<Query.GetAllOrdersQuery>
{
    public GetAllProductsInWishlistValidator()
    {
        // SortType and IsSortASC must either both exist or not exist
        RuleFor(x => x.FilterParams)
            .Must(filter => (filter.SortType != null && filter.IsSortASC != null) || (filter.SortType == null && filter.IsSortASC == null))
            .WithMessage("SortType and IsSortASC must either both be provided or both be null.");

        // MinValue and MaxValue must either both exist or not exist
        RuleFor(x => x.FilterParams)
            .Must(filter => (filter.MinValue != null && filter.MaxValue != null) || (filter.MinValue == null && filter.MaxValue == null))
            .WithMessage("MinValue and MaxValue must either both be provided or both be null.");
    }
}