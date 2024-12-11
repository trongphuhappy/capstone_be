using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.DTOs.StatisticDTOs;
using Neighbor.Contract.Services.Statistic;
using Neighbor.Domain.Abstraction.Dappers;
using static Neighbor.Domain.Exceptions.LessorException;

namespace Neighbor.Application.UseCases.V2.Queries.Statistic;

public sealed class GetTotalBoxLessorQueryHandler : IQueryHandler<Query.GetTotalBoxLessorQuery, Success<List<BoxLessorDTO>>>
{
    private readonly IDPUnitOfWork _dpUnitOfWork;

    public GetTotalBoxLessorQueryHandler(IDPUnitOfWork dpUnitOfWork)
    {
        _dpUnitOfWork = dpUnitOfWork;
    }

    public async Task<Result<Success<List<BoxLessorDTO>>>> Handle(Query.GetTotalBoxLessorQuery request, CancellationToken cancellationToken)
    {
        // Get lessorId by accountId
        var lessor = await _dpUnitOfWork.LessorRepositories.GetLessorByUserIdAsync(request.AccountId, new[] { "l.Id" });
        if (lessor == null) throw new LessorNotRegisterdException();
        var result = await _dpUnitOfWork.StatisticRepositories.GetTotalBoxLessorAsync(lessor.Id);
        return Result.Success(new Success<List<BoxLessorDTO>>("", "", result));
    }
}
