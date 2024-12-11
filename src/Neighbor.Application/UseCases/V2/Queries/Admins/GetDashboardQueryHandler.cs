using AutoMapper;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Contract.Services.Admins;
using Neighbor.Domain.Abstraction.Dappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Neighbor.Contract.Services.Admins.Response;

namespace Neighbor.Application.UseCases.V1.Queries.Admin
{
    public sealed class GetDashboardQueryHandler : IQueryHandler<Query.GetDashboardQuery, Success<DashboardResponse>>
    {
        private readonly IDPUnitOfWork _dpUnitOfWork;
        private readonly IMapper _mapper;

        public GetDashboardQueryHandler(IDPUnitOfWork dpUnitOfWork, IMapper mapper)
        {
            _dpUnitOfWork = dpUnitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<Success<DashboardResponse>>> Handle(Query.GetDashboardQuery request, CancellationToken cancellationToken)
        {
            //Count total for dashboard
            double totalRevenue = await _dpUnitOfWork.OrderRepositories.GetTotalRevenue();
            int totalUsers = await _dpUnitOfWork.AccountRepositories.CountAllUsers();

            //Calculate amount revenue in year
            Dictionary<int, double> listRevenueInYear = await _dpUnitOfWork.OrderRepositories.CountAmountInYear(request.Year);
            var listMonthNames = new List<string>
            {
                "January", "February", "March", "April", "May", "June",
                "July", "August", "September", "October", "November", "December"
            };

            //Return result
            var result = new DashboardResponse(totalUsers, totalRevenue, listMonthNames, listRevenueInYear.Values.ToList());
            return Result.Success(new Success<DashboardResponse>(MessagesList.GetDashboardForTotalSuccess.GetMessage().Code, MessagesList.GetDashboardForTotalSuccess.GetMessage().Message, result));

        }
    }
}
