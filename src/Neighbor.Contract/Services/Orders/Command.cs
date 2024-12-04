
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Enumarations.Order;
namespace Neighbor.Contract.Services.Orders;
public static class Command
{
    public record CreateOrderBankingCommand(Guid AccountId, Guid ProductId, DateTime RentTime, DateTime ReturnTime) : ICommand;
    public record OrderSuccessCommand(long OrderId) : ICommand<Success<Response.OrderSuccess>>;
    public record OrderFailCommand(long OrderId) : ICommand<Success<Response.OrderFail>>;
    public record UserConfirmOrderCommand(Guid AccountId, Guid OrderId, bool IsApproved, string? RejectReason) : ICommand;
    public record UserReportOrderCommand(Guid AccountId, Guid OrderId, string UserReport) : ICommand;
    public record AdminConfirmOrderCommand(Guid OrderId, bool IsApproved, string? RejectReason) : ICommand;

    public record LessorConfirmOrderCommand(Guid AccountId, Guid OrderId, bool IsApproved, string? RejectReason) : ICommand;
}

