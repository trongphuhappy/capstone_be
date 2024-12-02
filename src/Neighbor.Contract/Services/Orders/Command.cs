
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Enumarations.Order;
namespace Neighbor.Contract.Services.Orders;
public static class Command
{
    public record CreateOrderBankingCommand(Guid AccountId, Guid ProductId, DateTime RentTime, DateTime ReturnTime) : ICommand;
    public record OrderSuccessCommand(long OrderId) : ICommand<Success<Response.OrderSuccess>>;
    public record OrderFailCommand(long OrderId) : ICommand;

    public record UserConfirmOrderCommand(Guid ProductId, OrderStatusType OrderStatus, string? RejectReason) : ICommand;
    public record LessorConfirmOrderCommand(Guid ProductId, OrderStatusType OrderStatus, string? RejectReason) : ICommand;
}

