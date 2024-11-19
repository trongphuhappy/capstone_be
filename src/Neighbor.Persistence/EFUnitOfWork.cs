using Neighbor.Domain.Abstraction.EntitiyFramework;
using Neighbor.Domain.Abstraction.EntitiyFramework.Repositories;

namespace Neighbor.Persistence;
public class EFUnitOfWork : IEFUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public EFUnitOfWork(ApplicationDbContext context, IAccountRepository accountRepository)
    {
        _context = context;
        AccountRepository = accountRepository;
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
          => await _context.DisposeAsync();

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    => await _context.SaveChangesAsync();

    public IAccountRepository AccountRepository { get; }
}