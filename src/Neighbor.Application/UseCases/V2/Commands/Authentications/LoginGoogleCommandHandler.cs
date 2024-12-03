using MediatR;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Services.Authentications;
using Neighbor.Contract.Enumarations.Authentication;
using static Neighbor.Domain.Exceptions.AuthenticationException;
using Neighbor.Domain.Abstraction.EntitiyFramework;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Settings;
using Microsoft.Extensions.Options;

namespace Neighbor.Application.UseCases.V2.Commands.Authentications;

public sealed class LoginGoogleCommandHandler : ICommandHandler<Command.LoginGoogleCommand, Response.LoginResponse>
{
    private readonly IGoogleOAuthService _googleOAuthService;
    private readonly IEFUnitOfWork _efUnitOfWork;
    private readonly IPublisher _publisher;
    private readonly ITokenGeneratorService _tokenGeneratorService;
    private readonly UserSetting _userSetting;

    public LoginGoogleCommandHandler
        (IGoogleOAuthService googleOAuthService,
        IEFUnitOfWork efUnitOfWork,
        IPublisher publisher,
        ITokenGeneratorService tokenGeneratorService,
        IOptions<UserSetting> userConfiguration
        )
    {
        _googleOAuthService = googleOAuthService;
        _efUnitOfWork = efUnitOfWork;
        _publisher = publisher;
        _tokenGeneratorService = tokenGeneratorService;
        _userSetting = userConfiguration.Value;
    }

    public async Task<Result<Response.LoginResponse>> Handle(Command.LoginGoogleCommand request, CancellationToken cancellationToken)
    {
        // Get info user from access token Google
        var googleUserInfo = await _googleOAuthService.ValidateTokenAsync(request.AccessTokenGoogle);
        // If return == null => Exception
        if (googleUserInfo == null) throw new LoginGoogleFailException();
        // Check email have exit
        var account = await _efUnitOfWork.AccountRepository.GetAccountByEmailAsync(googleUserInfo.Email);
        // If have not account => Register account with type login Google
        if (account == null)
        {
            // Create object account member
            var accountMember = Domain.Entities.Account.CreateMemberAccountGoogle
                (googleUserInfo.Name, googleUserInfo.Name, googleUserInfo.Email, _userSetting.DefaultMaleAvatar, GenderType.Male);
            _efUnitOfWork.AccountRepository.Add(accountMember);
            // Save account
            await _efUnitOfWork.SaveChangesAsync();
            // Send mail when created success
            await Task.WhenAll(
                _publisher.Publish(new DomainEvent.UserCreatedWithGoogle(Guid.NewGuid(), googleUserInfo.Email),
                cancellationToken)
            );

            // Generate accessToken and refreshToken
            var accessToken = _tokenGeneratorService.GenerateAccessToken(accountMember.Id, (int)accountMember.RoleUserId);
            var refrehsToken = _tokenGeneratorService.GenerateRefreshToken(accountMember.Id, (int)accountMember.RoleUserId);

            return Result.Success
                (new Response.LoginResponse
                (accountMember.Id,
                accountMember.FirstName,
                accountMember.LastName,
                accountMember.CropAvatarUrl,
                accountMember.FullAvatarUrl,
                (int)accountMember.RoleUserId,
                accessToken,
                refrehsToken));
        }
        else
        {
            // If have account, check account not type Google
            if (account.LoginType != LoginType.Google) throw new AccountRegisteredAnotherMethodException();

            // If account banned
            if (account.IsDeleted == true) throw new AccountBanned();

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
}