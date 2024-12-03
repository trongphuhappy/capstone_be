using Neighbor.Domain.Entities;

namespace Neighbor.Domain.Abstraction.EntitiyFramework.Repositories;

public interface IAccountRepository : IRepositoryBase<Account, Guid>
{
    Task<Account> GetAccountByEmailAsync(string email);
}
