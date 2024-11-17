using MediatR;

namespace Neighbor.Contract.Abstractions.Message;

public interface IDomainEventHandler<TEvent> : INotificationHandler<TEvent>
    where TEvent : IDomainEvent
{
}
