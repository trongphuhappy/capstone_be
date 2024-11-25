using Neighbor.Domain.Abstraction.EntitiyFramework.Repositories;
using Neighbor.Domain.Entities;

namespace Neighbor.Persistence.Repositories;

public class WishlistRepository(ApplicationDbContext context) : RepositoryBase<Wishlist, Guid>(context), IWishlistRepository
{
}
