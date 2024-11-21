using Neighbor.Contract.Enumarations.MessagesList;

namespace Neighbor.Domain.Exceptions;

public static class AdminException
{
    public sealed class BanWithNoReasonException : BadRequestException
    {
        public BanWithNoReasonException()
            : base(MessagesList.AdminBanWithNoReasonException.GetMessage().Message,
                   MessagesList.AdminBanWithNoReasonException.GetMessage().Code)
        { }
    }
}
