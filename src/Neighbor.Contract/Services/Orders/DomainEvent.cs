using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Enumarations.Order;

namespace Neighbor.Contract.Services.Orders;

public static class DomainEvent
{
    public record NotiLessorOrderSuccess(Guid Id, string Email, string ProductName, string DeliveryAddress, DateTime MeetingDate, string UserEmail) : IDomainEvent;
    public record NotiUserOrderSuccess(Guid Id, string Email, string ProductName, string DeliveryAddress, DateTime MeetingDate) : IDomainEvent;
    public record NotiLessorAboutUserApprovedOrderSuccess(Guid Id, string Email, string ProductName) : IDomainEvent;
    public record NotiLessorAboutUserRejectedOrderSuccess(Guid Id, string Email, string ProductName, string RejectReason) : IDomainEvent;

    public record NotiUserAboutLessorApprovedOrderSuccess(Guid Id, string Email, string ProductName, OrderStatusType OrderStatus) : IDomainEvent;
    public record NotiUserAboutLessorRejectedOrderSuccess(Guid Id, string Email, string ProductName, string RejectReason) : IDomainEvent;

    public record NotiLessorAboutUserReportedOrderSuccess(Guid Id, string Email, string ProductName, string UserReport) : IDomainEvent;

    public record NotiLessorAboutAdminApprovedReportedOrderSuccess(Guid Id, string Email, string ProductName) : IDomainEvent;

    public record NotiLessorAboutAdminRejectedReportedOrderSuccess(Guid Id, string Email, string ProductName, string RejectReason) : IDomainEvent;

    public record NotiUserAboutAdminApprovedReportedOrderSuccess(Guid Id, string Email, string ProductName) : IDomainEvent;

    public record NotiUserAboutAdminRejectedReportedOrderSuccess(Guid Id, string Email, string ProductName, string RejectReason) : IDomainEvent;




}
