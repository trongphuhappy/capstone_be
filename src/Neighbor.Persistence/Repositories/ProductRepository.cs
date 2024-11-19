using Neighbor.Domain.Abstraction.EntitiyFramework.Repositories;
using Neighbor.Domain.Entities;

namespace Neighbor.Persistence.Repositories;

public class ProductRepository(ApplicationDbContext context) : RepositoryBase<Product, Guid>(context), IProductRepository
{
}
