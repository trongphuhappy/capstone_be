
using Microsoft.AspNetCore.Http;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.DTOs.ProductDTOs;
using Neighbor.Contract.Enumarations.Order;
using Neighbor.Contract.Enumarations.Product;
namespace Neighbor.Contract.Services.Orders;
public static class Command
{
    public record CreateOrderBankingCommand(Guid AccountId, Guid ProductId, DateTime RentTime, DateTime ReturnTime) : ICommand;

    public record UserConfirmOrderCommand(Guid ProductId, OrderStatusType OrderStatus, string? RejectReason) : ICommand;

    public record LessorConfirmOrderCommand(Guid ProductId, OrderStatusType OrderStatus, string? RejectReason) : ICommand;
}

