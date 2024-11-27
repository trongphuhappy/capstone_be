﻿using FluentValidation;
using Neighbor.Contract.Services.Orders;

namespace Neighbor.Contract.Services.Orders.Validators;

internal class LessorConfirmOrderValidator : AbstractValidator<Command.LessorConfirmOrderCommand>
{
    public LessorConfirmOrderValidator()
    {
        RuleFor(x => x.ProductId).NotNull().NotEmpty();
        RuleFor(x => x.OrderStatus).NotNull().NotEmpty();
    }
}