using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Services.Feedbacks;
using Neighbor.Domain.Entities;

namespace Neighbor.Domain.Abstraction.Dappers.Repositories;

public interface IFeedbackRepository : IGenericRepository<Domain.Entities.Feedback>
{
    Task<PagedResult<Feedback>> GetPagedAsync(int pageIndex, int pageSize, Filter.FeedbackFilter filterParams, string[] selectedColumns);
}
