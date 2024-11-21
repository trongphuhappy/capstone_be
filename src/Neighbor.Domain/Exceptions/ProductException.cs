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

    public sealed class ProductRejectNoReasonException : NotFoundException
    {
        public ProductRejectNoReasonException()
            : base(MessagesList.ProductRejectNoReasonException.GetMessage().Message,
                   MessagesList.ProductRejectNoReasonException.GetMessage().Code)
        { }
    }
}
