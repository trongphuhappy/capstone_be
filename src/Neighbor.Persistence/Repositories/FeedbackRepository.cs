using Microsoft.EntityFrameworkCore;
using Neighbor.Domain.Abstraction.EntitiyFramework.Repositories;
using Neighbor.Domain.Entities;

namespace Neighbor.Persistence.Repositories;

public class FeedbackRepository(ApplicationDbContext context) : RepositoryBase<Feedback, Guid>(context), IFeedbackRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Feedback> GetFeedbackByIdAsync(Guid feedbackId)
    {
        return await _context.Feedbacks
            .Include(f => f.Order)
            .FirstOrDefaultAsync(f => f.Id == feedbackId);
    }

    public async Task<Feedback> GetFeedbackByAccountId(Guid accountId)
    {
        return await _context.Feedbacks
            .Include(f => f.Order)
            .FirstOrDefaultAsync(f => f.AccountId == accountId);
    }
    public async Task<List<Feedback>> GetFeedbacksByOrderIdAsync(Guid orderId)
    {
        return await _context.Feedbacks
            .Include(f => f.Order)
            .Where(f => f.OrderId == orderId)
            .ToListAsync();
    }
}
