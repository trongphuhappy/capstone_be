using Neighbor.Contract.Enumarations.MessagesList;

namespace Neighbor.Domain.Exceptions;

public static class AuthenticationException
{
    public sealed class EmailExistException : BadRequestException
    {
        public EmailExistException()
            : base(MessagesList.AuthEmailExistException.GetMessage().Message,
                   MessagesList.AuthEmailExistException.GetMessage().Code)
        { }
    }

    public sealed class RegisterFailureException : BadRequestException
    {
        public RegisterFailureException()
            : base(MessagesList.AuthRegisterFailure.GetMessage().Message,
                   MessagesList.AuthRegisterFailure.GetMessage().Code)
        { }
    }
}
