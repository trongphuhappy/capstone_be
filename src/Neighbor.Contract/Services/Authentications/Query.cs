using Neighbor.Contract.Abstractions.Message;

namespace Neighbor.Contract.Services.Authentications;

public static class Query
{
    public record LoginQuery(string Email, string Password) : IQuery<Response.LoginResponse>;
}
