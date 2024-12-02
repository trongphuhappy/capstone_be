using Neighbor.Domain.Entities;

namespace Neighbor.Domain.Abstraction.EntitiyFramework.Repositories;

public interface IWalletRepository : IRepositoryBase<Wallet, Guid>
{
    Task<Wallet> GetWalletByLessorId(Guid lessorId);
}
