using Neighbor.Contract.Enumarations.MessagesList;

namespace Neighbor.Domain.Exceptions;

public static class ProductException
{
    public sealed class ProductNotFoundException : NotFoundException
    {
        public ProductNotFoundException()
            : base(MessagesList.ProductNotFoundException.GetMessage().Message,
                   MessagesList.ProductNotFoundException.GetMessage().Code)
        { }
    }

    public sealed class ProductNotFoundAnyException : NotFoundException
    {
        public ProductNotFoundAnyException()
            : base(MessagesList.ProductNotFoundAnyException.GetMessage().Message,
                   MessagesList.ProductNotFoundAnyException.GetMessage().Code)
        { }
    }

    public sealed class ProductRejectNoReasonException : BadRequestException
    {
        public ProductRejectNoReasonException()
            : base(MessagesList.ProductRejectNoReasonException.GetMessage().Message,
                   MessagesList.ProductRejectNoReasonException.GetMessage().Code)
        { }
    }

    public sealed class ProductHasAlreadyRejectedException : BadRequestException
    {
        public ProductHasAlreadyRejectedException()
            : base(MessagesList.ProductHasAlreadyRejectedException.GetMessage().Message,
                   MessagesList.ProductHasAlreadyRejectedException.GetMessage().Code)
        { }
    }

    public sealed class ProductHasAlreadyApprovedException : BadRequestException
    {
        public ProductHasAlreadyApprovedException()
            : base(MessagesList.ProductHasAlreadyApprovedException.GetMessage().Message,
                   MessagesList.ProductHasAlreadyApprovedException.GetMessage().Code)
        { }
    }

    public sealed class CanNotAddToWishlistBecauseProductBelongsToUserException : BadRequestException
    {
        public CanNotAddToWishlistBecauseProductBelongsToUserException()
            : base(MessagesList.ProductCanNotAddToWishlistBecauseProductBelongsToUserException.GetMessage().Message,
                   MessagesList.ProductCanNotAddToWishlistBecauseProductBelongsToUserException.GetMessage().Code)
        { }
    }
}
