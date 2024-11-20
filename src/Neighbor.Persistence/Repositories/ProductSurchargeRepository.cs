using Neighbor.Domain.Abstraction.EntitiyFramework.Repositories;
using Neighbor.Domain.Entities;

namespace Neighbor.Persistence.Repositories;

public class ProductSurchargeRepository(ApplicationDbContext context) : RepositoryBase<ProductSurcharge, Guid>(context), IProductSurchargeRepository
{
}
