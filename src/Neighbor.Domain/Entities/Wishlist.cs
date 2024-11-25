using Neighbor.Domain.Abstraction.Entities;

namespace Neighbor.Domain.Entities;

public class Wishlist : DomainEntity<Guid>
{
    public Wishlist()
    {
    }

    public Wishlist(Guid accountId, Guid productId)
    {
        AccountId = accountId;
        ProductId = productId;
    }
    public Guid? AccountId { get; private set; }
    public virtual Account? Account { get; private set; }
    public Guid? ProductId { get; private set; }
    public virtual Product? Product { get; private set; }

    public static Wishlist AddProductToWishlist(Guid accountId, Guid productId)
    {
        return new Wishlist(accountId, productId);
    }
}
