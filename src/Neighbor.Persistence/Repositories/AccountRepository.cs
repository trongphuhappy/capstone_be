using Microsoft.EntityFrameworkCore;
using Neighbor.Domain.Abstraction.EntitiyFramework.Repositories;
using Neighbor.Domain.Entities;

namespace Neighbor.Persistence.Repositories;

public class AccountRepository(ApplicationDbContext context) : RepositoryBase<Account, Guid>(context), IAccountRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Account> GetAccountByEmailAsync(string email)
    {
        return await _context.Accounts.FirstOrDefaultAsync(account => account.Email == email);
    }
}
