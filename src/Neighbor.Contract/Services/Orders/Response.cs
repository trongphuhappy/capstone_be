using Neighbor.Contract.DTOs.OrderDTOs;
using Neighbor.Contract.DTOs.ProductDTOs;
using Neighbor.Contract.Enumarations.Order;
using Neighbor.Contract.Enumarations.Product;
using static Neighbor.Contract.DTOs.ProductDTOs.ProductDTO;

namespace Neighbor.Contract.Services.Orders;

public static class Response
{
    public record OrderResponse(Guid Id, DateTime RentTime, DateTime ReturnTime, string DeliveryAddress, double OrderValue, OrderStatusType OrderStatus, string? UserReasonReject, string? LessorReasonReject, bool IsConflict, DateTime CreatedDate, ProductResponseDTO Product, UserDTO User, LessorDTO Lessor);
}
