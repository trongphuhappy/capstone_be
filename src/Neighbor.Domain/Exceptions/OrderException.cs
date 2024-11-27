using Neighbor.Contract.Enumarations.MessagesList;

namespace Neighbor.Domain.Exceptions;

public static class OrderException
{
    public sealed class ProductBelongsToUserException : BadRequestException
    {
        public ProductBelongsToUserException()
            : base(MessagesList.OrderProductBelongsToUserException.GetMessage().Message,
                   MessagesList.OrderProductBelongsToUserException.GetMessage().Code)
        { }
    }

    public sealed class ProductAlreadyOrderedByUserException : BadRequestException
    {
        public ProductAlreadyOrderedByUserException()
                : base(MessagesList.OrderProductAlreadyOrderedByUserException.GetMessage().Message,
                       MessagesList.OrderProductAlreadyOrderedByUserException.GetMessage().Code)
        { }
    }
}
