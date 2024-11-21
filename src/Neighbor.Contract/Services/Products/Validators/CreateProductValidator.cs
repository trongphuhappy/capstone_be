using FluentValidation;
using Neighbor.Contract.Services.Products;

namespace Neighbor.Contract.Services.Products.Validators;

internal class CreateProductValidator : AbstractValidator<Command.CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name).NotNull().NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Value).NotNull().NotEmpty();
        RuleFor(x => x.Price).NotNull().NotEmpty();
        RuleFor(x => x.Policies).NotNull().NotEmpty();
        RuleFor(x => x.CategoryId).NotNull().NotEmpty();   
        RuleFor(x => x.UserId).NotNull().NotEmpty();
        RuleFor(x => x.ProductImages).NotNull().NotEmpty();
    }
}
