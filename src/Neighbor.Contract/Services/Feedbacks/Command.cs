using Neighbor.Contract.Abstractions.Message;

namespace Neighbor.Contract.Services.Feedbacks;
public static class Command
{
    public record CreateFeedbackCommand
        (Guid AccountId, Guid OrderId, Guid ProductId, string Content)
        : ICommand;
}
