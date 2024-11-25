using Neighbor.Contract.Abstractions.Message;

namespace Neighbor.Contract.Services.Members;

public static class DomainEvent
{
    public record UserEmailChanged(Guid Id, Guid UserId, string Email) : IDomainEvent;
}
