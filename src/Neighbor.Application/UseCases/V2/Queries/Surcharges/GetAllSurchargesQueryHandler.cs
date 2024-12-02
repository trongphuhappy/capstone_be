using AutoMapper;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Contract.Services.Surcharges;
using Neighbor.Domain.Abstraction.Dappers;

namespace Neighbor.Application.UseCases.V2.Queries.Surcharges;

public sealed class GetAllSurchargesQueryHandler : IQueryHandler<Query.GetAllSurchargesQuery, Success<PagedResult<Response.SurchargeResponse>>>
{
    private readonly IDPUnitOfWork _dpUnitOfWork;
    private readonly IMapper _mapper;

    public GetAllSurchargesQueryHandler(IDPUnitOfWork dpUnitOfWork, IMapper mapper)
    {
        _dpUnitOfWork = dpUnitOfWork;
        _mapper = mapper;
    }
    public async Task<Result<Success<PagedResult<Response.SurchargeResponse>>>> Handle(Query.GetAllSurchargesQuery request, CancellationToken cancellationToken)
    {
        //Find all Surcharges
        var listSurcharges = await _dpUnitOfWork.SurchargeRepositories.GetPagedAsync(request.PageIndex, request.PageSize, request.FilterParams, request.SelectedColumns);
        //Mapping Surcharge to SurchargeResponse
        var result = _mapper.Map<PagedResult<Response.SurchargeResponse>>(listSurcharges);
        //Check if List Surcharges is empty
        if (listSurcharges.Items.Count == 0)
        {
            return Result.Success(new Success<PagedResult<Response.SurchargeResponse>>(MessagesList.SurchargeNotFoundAnyException.GetMessage().Code, MessagesList.SurchargeNotFoundAnyException.GetMessage().Message, result));
        }
        //Return result
        return Result.Success(new Success<PagedResult<Response.SurchargeResponse>>(MessagesList.SurchargeGetAllSurchargesSuccess.GetMessage().Code, MessagesList.SurchargeGetAllSurchargesSuccess.GetMessage().Message, result));
    }
}
