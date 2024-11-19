using Neighbor.Contract.Enumarations.MessagesList;

namespace Neighbor.Domain.Exceptions;

public static class SurchargeException
{
    public sealed class SurchargeNotFoundException : NotFoundException
    {
        public SurchargeNotFoundException()
            : base(MessagesList.SurchargeNotFoundAnyException.GetMessage().Message,
                   MessagesList.SurchargeNotFoundAnyException.GetMessage().Code)
        { }
    }
}
