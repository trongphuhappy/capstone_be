using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Services.Members;
using Neighbor.Domain.Abstraction.Dappers;

namespace Neighbor.Application.UseCases.V2.Queries.Members;

public sealed class CheckLessorExistQueryHandler : IQueryHandler<Query.CheckLessorExistQuery, Success<bool>>
{
    private readonly IDPUnitOfWork _dpUnitOfWork;

    public CheckLessorExistQueryHandler(IDPUnitOfWork dpUnitOfWork)
    {
        _dpUnitOfWork = dpUnitOfWork;
    }

    public async Task<Result<Success<bool>>> Handle(Query.CheckLessorExistQuery request, CancellationToken cancellationToken)
    {
        var result = await _dpUnitOfWork.LessorRepositories.LessorExistByAccountIdAsync(request.UserId);
        return Result.Success(new Success<bool>("", "", result));
    }
}
