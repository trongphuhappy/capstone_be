using Microsoft.Extensions.Options;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Services.Authentications;
using Neighbor.Contract.Settings;

namespace Neighbor.Application.UseCases.V2.Events;

public sealed class SendEmailWhenUserChangedEventHandler
    : IDomainEventHandler<DomainEvent.UserRegistedWithLocal>,
    IDomainEventHandler<DomainEvent.UserVerifiedEmailRegist>
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
            {"Link", $"{_clientSetting.Url}{_clientSetting.VerifyEmail}/{notification.Email}"}
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
}
