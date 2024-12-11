using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.DTOs.OrderDTOs;
using Neighbor.Contract.DTOs.StatisticDTOs;
using Neighbor.Contract.Services.Statistic;
using Neighbor.Domain.Abstraction.Dappers;
using static Neighbor.Domain.Exceptions.LessorException;

namespace Neighbor.Application.UseCases.V2.Queries.Statistic;

public sealed class GetCountOrderByLessorQueryHandler : IQueryHandler<Query.GetCountOrderByLessorQuery, Success<IEnumerable<OrderStatisticsDTO>>>
{
    private readonly IDPUnitOfWork _dpUnitOfWork;

    public GetCountOrderByLessorQueryHandler(IDPUnitOfWork dpUnitOfWork)
    {
        _dpUnitOfWork = dpUnitOfWork;
    }

    public async Task<Result<Success<IEnumerable<OrderStatisticsDTO>>>> Handle(Query.GetCountOrderByLessorQuery request, CancellationToken cancellationToken)
    {
        var lessor = await _dpUnitOfWork.LessorRepositories.GetLessorByUserIdAsync(request.AccountId, new[] { "l.Id" });
        if (lessor == null) throw new LessorNotRegisterdException();
        var result = await _dpUnitOfWork.StatisticRepositories.GetMonthlyOrderStatisticsByLessorAsync(lessor.Id);
        return Result.Success(new Success<IEnumerable<OrderStatisticsDTO>>("", "", result));
    }

}