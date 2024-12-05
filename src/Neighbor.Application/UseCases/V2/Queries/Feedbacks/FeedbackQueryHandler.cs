using AutoMapper;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.DTOs.FeedbackDTOs;
using Neighbor.Contract.Services.Feedbacks;
using Neighbor.Domain.Abstraction.Dappers;

namespace Neighbor.Application.UseCases.V2.Queries.Feedbacks;

public sealed class FeedbackQueryHandler : IQueryHandler<Query.GetFeedbacksQuery, Success<PagedResult<FeedbackDTO>>>
{
    private readonly IDPUnitOfWork _dpUnitOfWork;
    private readonly IMapper _mapper;

    public FeedbackQueryHandler(IDPUnitOfWork dpUnitOfWork, IMapper mapper)
    {
        _dpUnitOfWork = dpUnitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<Success<PagedResult<FeedbackDTO>>>> Handle(Query.GetFeedbacksQuery request, CancellationToken cancellationToken)
    {
        var result = await _dpUnitOfWork.FeedbackRepositories.GetPagedAsync(request.PageIndex, request.PageSize, request.FilterParams, request.SelectedColumns);

        var feedbackDtos = _mapper.Map<List<FeedbackDTO>>(result.Items);

        return Result.Success(new Success<PagedResult<FeedbackDTO>>("", "", new PagedResult<FeedbackDTO>(feedbackDtos, result.PageIndex, result.PageSize, result.TotalCount, result.TotalPages)));
    }
}
