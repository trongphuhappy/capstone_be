using Neighbor.Domain.Entities;

namespace Neighbor.Domain.Abstraction.Dappers.Repositories;

public interface ILessorRepository : IGenericRepository<Domain.Entities.Lessor>
{
    Task<Lessor> GetLessorByUserId(Guid userId);
}
