using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Services.Authentications;
using Neighbor.Domain.Abstraction.Dappers;

using static Neighbor.Domain.Exceptions.AuthenticationException;

namespace Neighbor.Application.UseCases.V2.Queries.Authentications;

public class RefreshTokenQueryHandler : IQueryHandler<Query.RefreshTokenQuery, Response.RefreshTokenResponse>
{
    private readonly IResponseCacheService _responseCacheService;
    private readonly ITokenGeneratorService _tokenGeneratorService;
    private readonly IDPUnitOfWork _dpUnitOfWork;

    public RefreshTokenQueryHandler
        (IResponseCacheService responseCacheService,
        ITokenGeneratorService tokenGeneratorService,
        IDPUnitOfWork dpUnitOfWork)
    {
        _responseCacheService = responseCacheService;
        _tokenGeneratorService = tokenGeneratorService;
        _dpUnitOfWork = dpUnitOfWork;
    }

    public async Task<Result<Response.RefreshTokenResponse>> Handle
        (Query.RefreshTokenQuery request, CancellationToken cancellationToken)
    {
        // Check refresh token and return userId get in refresh token decoded
        var userId = _tokenGeneratorService.ValidateAndGetUserIdFromRefreshToken(request.Token);
        // If return == null => Exception
        if (userId == null) throw new RefreshTokenNullException();

        var account = await _dpUnitOfWork.AccountRepositories.GetByIdAsync(Guid.Parse(userId));
        if (account == null) throw new AccountBanned();
        // Ban account
        if (account.IsDeleted == true) throw new AccountBanned();

        // Generate accesssToken and refreshToken
        var accessToken = _tokenGeneratorService.GenerateAccessToken(account.Id, (int)account.RoleUserId);
        var refrehsToken = _tokenGeneratorService.GenerateRefreshToken(account.Id, (int)account.RoleUserId);

        return Result.Success(
            new Response.RefreshTokenResponse
            (accessToken, refrehsToken));
    }
}

