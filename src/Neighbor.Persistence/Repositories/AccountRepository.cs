using Neighbor.Domain.Abstraction.EntitiyFramework.Repositories;
using Neighbor.Domain.Entities;

namespace Neighbor.Persistence.Repositories;

public class AccountRepository(ApplicationDbContext context) : RepositoryBase<Account, Guid>(context), IAccountRepository
{
}
