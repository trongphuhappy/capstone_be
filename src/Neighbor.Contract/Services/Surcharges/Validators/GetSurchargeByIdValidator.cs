using FluentValidation;

namespace Neighbor.Contract.Services.Surcharges.Validators;

internal class GetSurchargeByIdValidator : AbstractValidator<Query.GetSurchargeByIdQuery>
{
    public GetSurchargeByIdValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty();

    }
}
