using Neighbor.Domain.Abstraction.EntitiyFramework.Repositories;
using Neighbor.Domain.Entities;

namespace Neighbor.Persistence.Repositories;

public class OrderRepository(ApplicationDbContext context) : RepositoryBase<Order, Guid>(context), IOrderRepository
{
}
