using Neighbor.Contract.Enumarations.Order;

namespace Neighbor.Contract.Services.Orders;

public static class Filter
{
    public record OrderFilter(SortType? SortType, bool? IsSortASC, double? MinValue, double? MaxValue, string? DeliveryAddress, OrderStatusType? OrderStatus, OrderReportStatusType? OrderReportStatus, Guid? AccountUserId, Guid? AccountLessorId);
}
