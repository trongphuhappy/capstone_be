using Neighbor.Contract.Abstractions.Message;

namespace Neighbor.Contract.Services.Orders;

public static class DomainEvent
{
    public record NotiLessorOrderSuccess(Guid Id, string Email, string ProductName, string DeliveryAddress, DateTime MeetingDate, string UserEmail) : IDomainEvent;
    public record NotiUserOrderSuccess(Guid Id, string Email, string ProductName, string DeliveryAddress, DateTime MeetingDate) : IDomainEvent;

}
