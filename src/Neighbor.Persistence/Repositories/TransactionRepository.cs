using Neighbor.Domain.Abstraction.EntitiyFramework.Repositories;
using Neighbor.Domain.Entities;

namespace Neighbor.Persistence.Repositories;

public class TransactionRepository(ApplicationDbContext context) : RepositoryBase<Transaction, Guid>(context), ITransactionRepository
{
}
