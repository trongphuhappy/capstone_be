using Neighbor.Contract.Enumarations.MessagesList;

namespace Neighbor.Domain.Exceptions;

public static class SurchargeException
{
    public sealed class SurchargesNotFoundException : NotFoundException
    {
        public SurchargesNotFoundException()
            : base(MessagesList.SurchargeNotFoundAnyException.GetMessage().Message,
                   MessagesList.SurchargeNotFoundAnyException.GetMessage().Code)
        { }
    }

    public sealed class SurchargeNotFoundException : NotFoundException
    {
        public SurchargeNotFoundException()
            : base(MessagesList.SurchargeNotFoundException.GetMessage().Message,
                   MessagesList.SurchargeNotFoundException.GetMessage().Code)
        { }
    }
}
