using MediatR;

namespace Neighbor.Contract.Abstractions.Message;
public interface IDomainEvent : INotification
{
    public Guid Id { get; init; }
}
