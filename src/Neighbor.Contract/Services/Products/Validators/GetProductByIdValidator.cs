using FluentValidation;
using Neighbor.Contract.Services.Products;

namespace Neighbor.Contract.Services.Products.Validators;

internal class GetProductByIdValidator : AbstractValidator<Query.GetProductByIdQuery>
{
    public GetProductByIdValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty();
    }
}
