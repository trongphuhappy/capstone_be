using Microsoft.EntityFrameworkCore;
using Neighbor.Domain.Abstraction.EntitiyFramework.Repositories;
using Neighbor.Domain.Entities;

namespace Neighbor.Persistence.Repositories;

public class LessorRepository(ApplicationDbContext context) : RepositoryBase<Lessor, Guid>(context), ILessorRepository
{
    private readonly ApplicationDbContext _context = context;
    public async Task<Lessor> GetInformationLessorByAccountIdAsync(Guid accountId)
    {
        var result = await _context.Lessors.FirstOrDefaultAsync(lessor => lessor.AccountId == accountId);
        return result;
    }
}
