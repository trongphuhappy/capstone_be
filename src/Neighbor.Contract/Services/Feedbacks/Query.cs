using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.DTOs.FeedbackDTOs;
using static Neighbor.Contract.Services.Feedbacks.Filter;

namespace Neighbor.Contract.Services.Feedbacks;
public static class Query
{
    public record GetFeedbacksQuery(int PageIndex,
        int PageSize,
        FeedbackFilter FilterParams,
        string[] SelectedColumns) : IQuery<Success<PagedResult<FeedbackDTO>>>;
}