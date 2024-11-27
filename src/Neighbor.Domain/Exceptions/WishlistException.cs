using Neighbor.Contract.Enumarations.MessagesList;

namespace Neighbor.Domain.Exceptions;

public static class WishlistException
{
    public sealed class ProductBelongsToUserException : BadRequestException
    {
        public ProductBelongsToUserException()
            : base(MessagesList.WishlistProductBelongsToUserException.GetMessage().Message,
                   MessagesList.WishlistProductBelongsToUserException.GetMessage().Code)
        { }
    }
}
