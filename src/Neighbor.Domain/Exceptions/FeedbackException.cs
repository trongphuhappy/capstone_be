using Neighbor.Contract.Enumarations.MessagesList;

namespace Neighbor.Domain.Exceptions;

public static class FeedbackException
{
    public sealed class AccountHasFeedbackException : NotFoundException
    {
        public AccountHasFeedbackException()
            : base(MessagesList.AccountHasFeedbackException.GetMessage().Message,
                   MessagesList.AccountHasFeedbackException.GetMessage().Code)
        { }
    }
}

