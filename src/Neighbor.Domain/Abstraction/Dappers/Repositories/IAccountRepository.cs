using Neighbor.Domain.Entities;

namespace Neighbor.Domain.Abstraction.Dappers.Repositories;

public interface IAccountRepository : IGenericRepository<Domain.Entities.Account>
{
    Task<bool> EmailExistSystemAsync(string email);
    Task<bool>? AccountExistSystemAsync(Guid userId);
    Task<Account> GetByEmailAsync(string email);
}
