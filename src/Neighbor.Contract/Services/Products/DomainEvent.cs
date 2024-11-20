using Neighbor.Contract.Abstractions.Message;

namespace Neighbor.Contract.Services.Products;

public static class DomainEvent
{
    public record ProductHasBeenApproved(Guid Id, string Email, string ProductName) : IDomainEvent;
    public record ProductHasBeenRejected(Guid Id, string Email, string ProductName, string RejectReason) : IDomainEvent;
}
