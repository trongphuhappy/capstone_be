using Neighbor.Domain.Entities;

namespace Neighbor.Domain.Abstraction.Dappers.Repositories;

public interface IWalletRepository : IGenericRepository<Wallet>
{
    Task<Wallet> GetWalletByLessorId(Guid lessorId, string[] selectedColumns = null);
}
