using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.DTOs.AuthenticationDTOs;
using Neighbor.Contract.Enumarations.Authentication;
using Neighbor.Contract.Services.Authentications;
using Neighbor.Domain.Abstraction.Dappers;
using static Neighbor.Domain.Exceptions.AuthenticationException;

namespace Neighbor.Application.UseCases.V2.Queries.Authentications;

public sealed class LoginQueryHandler : IQueryHandler<Query.LoginQuery, Response.LoginResponse>
{
    private readonly ITokenGeneratorService _tokenGeneratorService;
    private readonly IDPUnitOfWork _dpUnitOfWork;
    private readonly IPasswordHashService _passwordHashService;

    public LoginQueryHandler
        (ITokenGeneratorService tokenGeneratorService,
        IDPUnitOfWork dpUnitOfWork,
        IPasswordHashService passwordHashService)
    {
        _tokenGeneratorService = tokenGeneratorService;
        _dpUnitOfWork = dpUnitOfWork;
        _passwordHashService = passwordHashService;
    }

    public async Task<Result<Response.LoginResponse>>
        Handle(Query.LoginQuery request, CancellationToken cancellationToken)
    {
        var account = await _dpUnitOfWork.AccountRepositories.GetByEmailAsync(request.Email);
        // If account == null => Exception
        if (account == null) throw new EmailNotExistException();
        // If account have login type != Local => Exception
        if (account.LoginType != LoginType.Local)
            throw new AccountRegisteredAnotherMethodException();

        if (account.IsDeleted == true) throw new AccountBanned();

        // Check password have equal with password hashed
        var isVerifyPassword = _passwordHashService.VerifyPassword(request.Password, account.Password);
        // If password not match
        if (isVerifyPassword == false) throw new PasswordNotMatchException();

        // Generate accessToken and refreshToken
        var accessToken = _tokenGeneratorService.GenerateAccessToken(account.Id, (int)account.RoleUserId);
        var refrehsToken = _tokenGeneratorService.GenerateRefreshToken(account.Id, (int)account.RoleUserId);

        return Result.Success
            (new Response.LoginResponse
            (account.Id,
            account.FirstName,
            account.LastName,
            account.CropAvatarUrl,
            account.FullAvatarUrl,
            (int)account.RoleUserId,
            accessToken,
            refrehsToken));
    }
}
