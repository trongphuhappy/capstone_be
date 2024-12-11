using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using static Neighbor.Contract.Services.Admins.Response;

namespace Neighbor.Contract.Services.Admins;

public static class Query
{
    public record LoginQuery(string Email, string Password) : IQuery<LoginResponse>;
    public record RefreshTokenQuery
     (string Token) : IQuery<RefreshTokenResponse>;

    public record GetDashboardQuery(int Year) : IQuery<Success<DashboardResponse>>;
}
