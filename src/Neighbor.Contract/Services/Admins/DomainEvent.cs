using Neighbor.Contract.Abstractions.Message;

namespace Neighbor.Contract.Services.Admins;

public static class DomainEvent
{
    public record AccountHasBeenBanned(Guid Id, string Email, string BanReason) : IDomainEvent;
    public record AccountHasBeenUnbanned(Guid Id, string Email) : IDomainEvent;
}
