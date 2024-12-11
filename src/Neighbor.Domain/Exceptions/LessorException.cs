using Neighbor.Contract.Enumarations.MessagesList;

namespace Neighbor.Domain.Exceptions;

public static class LessorException
{
    public sealed class LessorNotFoundException : NotFoundException
    {
        public LessorNotFoundException()
            : base(MessagesList.LessorNotFoundException.GetMessage().Message,
                   MessagesList.LessorNotFoundException.GetMessage().Code)
        { }
    }
    public sealed class LessorNotRegisterdException : NotFoundException
    {
        public LessorNotRegisterdException()
            : base(MessagesList.LessorNotRegisterdException.GetMessage().Message,
                   MessagesList.LessorNotRegisterdException.GetMessage().Code)
        { }
    }
}
