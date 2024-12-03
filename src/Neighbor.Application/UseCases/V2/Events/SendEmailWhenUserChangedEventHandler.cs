using Microsoft.Extensions.Options;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Services.Authentications;
using Neighbor.Contract.Settings;

namespace Neighbor.Application.UseCases.V2.Events;

public sealed class SendEmailWhenUserChangedEventHandler
    : IDomainEventHandler<DomainEvent.UserRegistedWithLocal>,
    IDomainEventHandler<DomainEvent.UserVerifiedEmailRegist>,
    IDomainEventHandler<Contract.Services.Members.DomainEvent.UserEmailChanged>,
    IDomainEventHandler<DomainEvent.UserOtpChanged>,
    IDomainEventHandler<DomainEvent.UserPasswordChanged>
{

    private readonly IEmailService _emailService;
    private readonly ClientSetting _clientSetting;

    public SendEmailWhenUserChangedEventHandler(IEmailService emailService,
        IOptions<ClientSetting> clientConfig)
    {
        _emailService = emailService;
        _clientSetting = clientConfig.Value;
    }

    public async Task Handle(DomainEvent.UserRegistedWithLocal notification, CancellationToken cancellationToken)
    {
        await _emailService.SendMailAsync
            (notification.Email,
            "Register Neighbor",
            "EmailRegister.html", new Dictionary<string, string> {
            { "ToEmail", notification.Email},
            {"LinkWeb", $"{_clientSetting.Url}"},
            {"LinkVerifyEmail", $"{_clientSetting.Url}{_clientSetting.VerifyEmail}/{notification.Email}"}
        });
    }

    public async Task Handle(DomainEvent.UserVerifiedEmailRegist notification, CancellationToken cancellationToken)
    {
        await _emailService.SendMailAsync
           (notification.Email,
           "VerifyEmail Neighbor",
           "EmailRegister.html", new Dictionary<string, string> {
            { "ToEmail", notification.Email},
            {"Link", $"{_clientSetting.Url}"}
       });
    }

    public async Task Handle(Contract.Services.Members.DomainEvent.UserEmailChanged notification, CancellationToken cancellationToken)
    {
        await _emailService.SendMailAsync
           (notification.Email,
           "Change email",
           "EmailUserChangeEmail.html", new Dictionary<string, string> {
            {"ToEmail", notification.Email},
               {"Link", $"{_clientSetting.Url}{_clientSetting.VerifyChangeEmail}/{notification.UserId}"}
       });
    }

    public async Task Handle(DomainEvent.UserCreatedWithGoogle notification, CancellationToken cancellationToken)
    {
        await _emailService.SendMailAsync
            (notification.Email,
            "Register with Google",
            "EmailRegister.html", new Dictionary<string, string> {
            { "ToEmail", notification.Email},
            {"LinkWeb", $"{_clientSetting.Url}"},
            {"LinkVerifyEmail", $"{_clientSetting.Url}{_clientSetting.VerifyEmail}/{notification.Email}"}
        });
    }

    public async Task Handle(DomainEvent.UserOtpChanged notification, CancellationToken cancellationToken)
    {
        await _emailService.SendMailAsync
           (notification.Email,
           "Forgot password PawFund",
           "EmailForgotPassword.html", new Dictionary<string, string> {
            {"ToEmail", notification.Email},
            {"Otp", notification.Otp}
       });
    }

    public async Task Handle(DomainEvent.UserPasswordChanged notification, CancellationToken cancellationToken)
    {
        await _emailService.SendMailAsync
           (notification.Email,
           "Change password",
           "EmailUserChangePassword.html", new Dictionary<string, string> {
            {"ToEmail", notification.Email},
               {"Link", $"{_clientSetting.Url}{_clientSetting.VerifyChangePassword}/{notification.Id}"}
       });
    }
}
