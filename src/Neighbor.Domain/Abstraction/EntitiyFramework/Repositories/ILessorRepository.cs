using Neighbor.Domain.Entities;

namespace Neighbor.Domain.Abstraction.EntitiyFramework.Repositories;

public interface ILessorRepository : IRepositoryBase<Lessor, Guid>
{
    Task<Lessor> GetInformationLessorByAccountIdAsync(Guid accountId);
}
