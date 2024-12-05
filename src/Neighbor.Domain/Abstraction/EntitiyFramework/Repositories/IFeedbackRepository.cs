using Neighbor.Domain.Entities;

namespace Neighbor.Domain.Abstraction.EntitiyFramework.Repositories;

public interface IFeedbackRepository : IRepositoryBase<Feedback, Guid>
{
    Task<Feedback> GetFeedbackByIdAsync(Guid feedbackId);
    Task<Feedback> GetFeedbackByAccountId(Guid accountId);
    Task<List<Feedback>> GetFeedbacksByOrderIdAsync(Guid orderId);
}
