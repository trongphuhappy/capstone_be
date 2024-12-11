using FluentValidation;
using Neighbor.Contract.Services.Admins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neighbor.Contract.Services.Admins.Validator
{
    public class GetDashboardValidator : AbstractValidator<Query.GetDashboardQuery>
    {
        public GetDashboardValidator()
        {
            RuleFor(x => x.Year).NotEmpty();
        }
    }
}
