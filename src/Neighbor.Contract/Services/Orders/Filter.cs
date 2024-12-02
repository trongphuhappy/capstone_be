using Neighbor.Contract.DTOs.OrderDTOs;
using Neighbor.Contract.Enumarations.Order;
using Neighbor.Contract.Enumarations.Product;

namespace Neighbor.Contract.Services.Orders;

public static class Filter
{
    public record OrderFilter(SortDTO? SortBy, double? MinValue, double? MaxValue, string? DeliveryAddress, OrderStatusType? OrderStatus, bool? IsConflict);

}
