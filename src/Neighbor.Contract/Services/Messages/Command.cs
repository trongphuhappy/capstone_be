using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.DTOs.MessageDTOs;

namespace Neighbor.Contract.Services.Messages;

public static class Command
{
    public record CreateMessageChatBoxCommand(string SessionId, string Content) : ICommand<Success<CreateMessageDTO>>;
}
