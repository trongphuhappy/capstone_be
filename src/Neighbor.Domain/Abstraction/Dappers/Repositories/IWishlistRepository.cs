using Neighbor.Domain.Entities;

namespace Neighbor.Domain.Abstraction.Dappers.Repositories;

public interface IWishlistRepository : IGenericRepository<Domain.Entities.Wishlist>
{
    Task<bool> IsProductExistInWishlist(Guid AccountId, Guid ProductId);
}
