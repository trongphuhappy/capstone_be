using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Enumarations.Authentication;

namespace Neighbor.Contract.Services.Admins;
public static class Command
{
    public record HandleUserCommand
        (Guid AccountId, bool IsDeleted, string? BanReason)
        : ICommand;

}
