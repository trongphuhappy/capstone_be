using AutoMapper;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.DTOs.MemberDTOs;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Contract.Services.Members;
using Neighbor.Domain.Abstraction.Dappers;
using static Neighbor.Domain.Exceptions.AuthenticationException;

namespace Neighbor.Application.UseCases.V2.Queries.Members;

public sealed class GetProfileQueryHandler : IQueryHandler<Query.GetProfileQuery, Success<ProfileDTO>>
{
    private readonly IDPUnitOfWork _dpUnitOfWork;
    private readonly IMapper _mapper;

    public GetProfileQueryHandler
        (IDPUnitOfWork dpUnitOfWork,
        IMapper mapper)
    {
        _dpUnitOfWork = dpUnitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<Success<ProfileDTO>>> Handle(Query.GetProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _dpUnitOfWork.AccountRepositories.GetByIdAsync(request.UserId);
        if (user.IsDeleted == true) throw new AccountBanned();
        
        var result = _mapper.Map<ProfileDTO>(user);
        return Result.Success(new Success<ProfileDTO>(MessagesList.GetProfileSuccessfully.GetMessage().Code,
            MessagesList.GetProfileSuccessfully.GetMessage().Message,
            result));
    }
}
