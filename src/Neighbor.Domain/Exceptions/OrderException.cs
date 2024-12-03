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

    public sealed class ProductOrderedByAnotherUserException : BadRequestException
    {
        public ProductOrderedByAnotherUserException()
                : base(MessagesList.OrderProductOrderedByAnotherUserException.GetMessage().Message,
                       MessagesList.OrderProductOrderedByAnotherUserException.GetMessage().Code)
        { }
    }

    public sealed class ProductNotApprovedByAdminException : BadRequestException
    {
        public ProductNotApprovedByAdminException()
                : base(MessagesList.OrderProductNotApprovedByAdminException.GetMessage().Message,
                       MessagesList.OrderProductNotApprovedByAdminException.GetMessage().Code)
        { }
    }

    public sealed class OrderNotFoundAnyException : NotFoundException
    {
        public OrderNotFoundAnyException()
                : base(MessagesList.OrderNotFoundAnyException.GetMessage().Message,
                       MessagesList.OrderNotFoundAnyException.GetMessage().Code)
        { }
    }

    public sealed class OrderNotFoundException : NotFoundException
    {
        public OrderNotFoundException()
                : base(MessagesList.OrderNotFoundException.GetMessage().Message,
                       MessagesList.OrderNotFoundException.GetMessage().Code)
        { }
    }

    public sealed class OrderHaveAlreadyConfirmedException : BadRequestException
    {
        public OrderHaveAlreadyConfirmedException()
                : base(MessagesList.OrderHaveAlreadyConfirmedException.GetMessage().Message,
                       MessagesList.OrderHaveAlreadyConfirmedException.GetMessage().Code)
        { }
    }

    public sealed class OrderRejectWithoutReasonException : BadRequestException
    {
        public OrderRejectWithoutReasonException()
                : base(MessagesList.OrderRejectWithoutReasonException.GetMessage().Message,
                       MessagesList.OrderRejectWithoutReasonException.GetMessage().Code)
        { }
    }

    public sealed class OrderNotBelongToUserException : BadRequestException
    {
        public OrderNotBelongToUserException()
                : base(MessagesList.OrderNotBelongToUserException.GetMessage().Message,
                       MessagesList.OrderNotBelongToUserException.GetMessage().Code)
        { }
    }

    public sealed class OrderUserHasNotConfirmException : BadRequestException
    {
        public OrderUserHasNotConfirmException()
                : base(MessagesList.OrderUserHasNotConfirmException.GetMessage().Message,
                       MessagesList.OrderUserHasNotConfirmException.GetMessage().Code)
        { }
    }
}
