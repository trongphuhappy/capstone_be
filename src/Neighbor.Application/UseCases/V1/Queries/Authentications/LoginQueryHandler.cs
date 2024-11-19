using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Services.Authentications;

namespace Neighbor.Application.UseCases.V1.Queries.Authentications;

public sealed class LoginQueryHandler : IQueryHandler<Query.LoginQuery, Response.LoginResponse>
{
    public Task<Result<Response.LoginResponse>> Handle(Query.LoginQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
