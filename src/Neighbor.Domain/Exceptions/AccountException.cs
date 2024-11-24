using Neighbor.Contract.Enumarations.MessagesList;

namespace Neighbor.Domain.Exceptions;

public static class AccountException
{
    public sealed class AccountNotFoundException : NotFoundException
    {
        public AccountNotFoundException()
            : base(MessagesList.AccountNotFoundException.GetMessage().Message,
                   MessagesList.AccountNotFoundException.GetMessage().Code)
        { }
    }

    public sealed class AccountHasAlreadyBannedException : BadRequestException
    {
        public AccountHasAlreadyBannedException()
            : base(MessagesList.AccountHasAlreadyBannedException.GetMessage().Message,
                   MessagesList.AccountHasAlreadyBannedException.GetMessage().Code)
        { }
    }

    public sealed class AccountHasAlreadyUnbannedException : BadRequestException
    {
        public AccountHasAlreadyUnbannedException()
            : base(MessagesList.AccountHasAlreadyUnbannedException.GetMessage().Message,
                   MessagesList.AccountHasAlreadyUnbannedException.GetMessage().Code)
        { }
    }

    public class AccountUpdateEmailExit : NotFoundException
    {
        public AccountUpdateEmailExit()
        : base(MessagesList.AccountEmailUpdateExit.GetMessage().Message,
               MessagesList.AccountEmailUpdateExit.GetMessage().Code)
        { }
    }
}
