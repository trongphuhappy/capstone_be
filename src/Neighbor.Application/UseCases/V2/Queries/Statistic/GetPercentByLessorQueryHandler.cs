using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.DTOs.OrderDTOs;
using Neighbor.Contract.Services.Statistic;
using Neighbor.Domain.Abstraction.Dappers;
using static Neighbor.Domain.Exceptions.LessorException;

namespace Neighbor.Application.UseCases.V2.Queries.Statistic;

public sealed class GetPercentByLessorQueryHandler : IQueryHandler<Query.GetPercentByLessorQuery, Success<OrderStatsDTO>>
{
    private readonly IDPUnitOfWork _dpUnitOfWork;

    public GetPercentByLessorQueryHandler(IDPUnitOfWork dpUnitOfWork)
    {
        _dpUnitOfWork = dpUnitOfWork;
    }

    public async Task<Result<Success<OrderStatsDTO>>> Handle(Query.GetPercentByLessorQuery request, CancellationToken cancellationToken)
    {
        var lessor = await _dpUnitOfWork.LessorRepositories.GetLessorByUserIdAsync(request.AccountId, new[] { "l.Id" });
        if (lessor == null) throw new LessorNotRegisterdException();
        var result = await _dpUnitOfWork.StatisticRepositories.GetPercentTotalOrder(lessor.Id);
        return Result.Success(new Success<OrderStatsDTO>("", "", result));
    }
}