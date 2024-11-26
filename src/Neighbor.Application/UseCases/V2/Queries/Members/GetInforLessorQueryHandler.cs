using AutoMapper;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.DTOs.MemberDTOs;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Contract.Services.Members;
using Neighbor.Domain.Abstraction.Dappers;
using static Neighbor.Domain.Exceptions.AuthenticationException;

namespace Neighbor.Application.UseCases.V2.Queries.Members;

public sealed class GetInforLessorQueryHandler : IQueryHandler<Query.GetInforLessorQuery, Success<LessorDTO>>
{
    private readonly IDPUnitOfWork _dpUnitOfWork;
    private readonly IMapper _mapper;

    public GetInforLessorQueryHandler
        (IDPUnitOfWork dpUnitOfWork,
        IMapper mapper)
    {
        _dpUnitOfWork = dpUnitOfWork;
        _mapper = mapper;
    }
    public async Task<Result<Success<LessorDTO>>> Handle(Query.GetInforLessorQuery request, CancellationToken cancellationToken)
    {
        var user = await _dpUnitOfWork.AccountRepositories.GetByIdAsync(request.UserId);
        // Check user have banned
        if (user.IsDeleted == true) throw new AccountBanned();
        var lessor = await _dpUnitOfWork.LessorRepositories.GetLessorByUserIdAsync(request.UserId);
        if (lessor == null)
        {
            return Result.Success(new Success<LessorDTO>(MessagesList.GetProfileSuccessfully.GetMessage().Code,
                MessagesList.GetProfileSuccessfully.GetMessage().Message,
                null));
        }
        var result = _mapper.Map<LessorDTO>(lessor);
        return Result.Success(new Success<LessorDTO>(MessagesList.GetProfileSuccessfully.GetMessage().Code,
            MessagesList.GetProfileSuccessfully.GetMessage().Message,
            result));
    }
}
