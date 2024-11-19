using Neighbor.Domain.Abstraction.Dappers;
using Neighbor.Domain.Abstraction.Dappers.Repositories;

namespace Neighbor.Infrastructure.Dapper;
public class DPUnitOfWork : IDPUnitOfWork
{
    public DPUnitOfWork(IAccountRepository accountRepository)
    {
        AccountRepositories = accountRepository;
    }
    public IAccountRepository AccountRepositories { get; }
}
