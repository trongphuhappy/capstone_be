using Neighbor.Domain.Entities;

namespace Neighbor.Domain.Abstraction.Dappers.Repositories;

public interface ILessorRepository : IGenericRepository<Domain.Entities.Lessor>
{
    Task<bool>? LessorExistByAccountIdAsync(Guid userId);
    Task<Lessor> GetLessorByUserIdAsync(Guid userId, string[] selectedColumns = null);
}
