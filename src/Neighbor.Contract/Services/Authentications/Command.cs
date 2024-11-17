using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Enumarations.Authentication;

namespace Neighbor.Contract.Services.Authentications;
public static class Command
{
    public record RegisterCommand
        (string FirstName,
        string LastName,
        string Email,
        string PhoneNumber,
        string Password,
        GenderType Gender)
        : ICommand;

    public record VerifyEmailCommand(string Email) : ICommand;
}
