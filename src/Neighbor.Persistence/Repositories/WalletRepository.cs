using Microsoft.EntityFrameworkCore;
using Neighbor.Domain.Abstraction.EntitiyFramework.Repositories;
using Neighbor.Domain.Entities;

namespace Neighbor.Persistence.Repositories;

public class WalletRepository(ApplicationDbContext context) : RepositoryBase<Wallet, Guid>(context), IWalletRepository
{
    private readonly ApplicationDbContext _context = context;
    public async Task<Wallet> GetWalletByLessorId(Guid lessorId)
    {
        return await _context.Wallets.FirstOrDefaultAsync(wallet => wallet.LessorId == lessorId);
    }
}
