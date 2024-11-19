using Neighbor.Domain.Entities;

namespace Neighbor.Domain.Abstraction.Dappers.Repositories;

public interface IProductRepository : IGenericRepository<Domain.Entities.Product>
{
    Task<bool> EmailExistSystemAsync(string email);
    Task<bool>? AccountExistSystemAsync(Guid userId);
    Task<Account> GetByEmailAsync(string email);
}
