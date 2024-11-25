using FluentValidation;
using Neighbor.Contract.Services.Products;

namespace Neighbor.Contract.Services.Products.Validators
{
    internal class GetAllProductsValidator : AbstractValidator<Query.GetAllProductsQuery>
    {
        public GetAllProductsValidator()
        {
            RuleFor(x => x.FilterParams)
                .Must(filter =>
                    !(filter.AccountUserId.HasValue && filter.AccountLessorId.HasValue) // Both cannot be filled
                    || (!filter.AccountUserId.HasValue && !filter.AccountLessorId.HasValue) // Neither is filled
                )
                .WithMessage("Only one of AccountUserId or AccountLessorId should be filled, or leave both empty.");
        }
    }
}
