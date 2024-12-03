using MediatR;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Services.Authentications;
using System.Security.Cryptography;
using System.Text.Json;
using Neighbor.Domain.Abstraction.EntitiyFramework;
using static Neighbor.Domain.Exceptions.AuthenticationException;
using Neighbor.Contract.Enumarations.Authentication;
using Neighbor.Contract.Enumarations.MessagesList;

namespace Neighbor.Application.UseCases.V2.Commands.Authentications;

public sealed class ForgotPasswordEmailCommandHandler : ICommandHandler<Command.ForgotPasswordEmailCommand>
{
    private readonly IEFUnitOfWork _efUnitOfWork;
    private readonly IPublisher _publisher;
    private readonly IResponseCacheService _responseCacheService;

    public ForgotPasswordEmailCommandHandler
        (IEFUnitOfWork efUnitOfWork,
        IPublisher publisher,
        IResponseCacheService responseCacheService)
    {
        _efUnitOfWork = efUnitOfWork;
        _publisher = publisher;
        _responseCacheService = responseCacheService;
    }
    /// <summary>
    /// Send email to create otp
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="EmailNotFoundException"></exception>

    public async Task<Result> Handle(Command.ForgotPasswordEmailCommand request, CancellationToken cancellationToken)
    {
        // Check email is in system
        var userInfo = await _efUnitOfWork.AccountRepository.GetAccountByEmailAsync(request.Email);
        // If user haven't system => Exception
        if (userInfo == null) throw new EmailNotExistException();
        // If user have type login != Local => Exception
        if (userInfo.LoginType != LoginType.Local)
            throw new EmailGoogleRegistedException();

        // Random OTP
        string otp = GenerateSecureOTP();

        // Save memory
        await _responseCacheService.SetCacheResponseAsync
            ($"forgotpassword_{request.Email}",
            JsonSerializer.Serialize(otp),
            TimeSpan.FromMinutes(15));

        // Send mail notification send otp
        await Task.WhenAll(
            _publisher.Publish(new DomainEvent.UserOtpChanged(Guid.NewGuid(), request.Email, otp), cancellationToken)
        );

        return Result.Success(new Success<string>(MessagesList.AuthForgotPasswordEmailSuccess.GetMessage().Code,
            MessagesList.AuthForgotPasswordEmailSuccess.GetMessage().Message, request.Email));
    }

    private static string GenerateSecureOTP()
    {
        var bytes = new byte[4];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }
        int otp = Math.Abs(BitConverter.ToInt32(bytes, 0)) % 90000 + 10000;
        return otp.ToString();
    }
}
