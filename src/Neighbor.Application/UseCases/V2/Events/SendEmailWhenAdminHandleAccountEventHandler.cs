using Microsoft.Extensions.Options;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Services.Admins;
using Neighbor.Contract.Settings;

namespace Neighbor.Application.UseCases.V2.Events;

public sealed class SendEmailWhenAdminHandleAccountEventHandler
    : IDomainEventHandler<DomainEvent.AccountHasBeenBanned>,
    IDomainEventHandler<DomainEvent.AccountHasBeenUnbanned>
{

    private readonly IEmailService _emailService;
    private readonly ClientSetting _clientSetting;

    public SendEmailWhenAdminHandleAccountEventHandler(IEmailService emailService,
        IOptions<ClientSetting> clientConfig)
    {
        _emailService = emailService;
        _clientSetting = clientConfig.Value;
    }

    public async Task Handle(DomainEvent.AccountHasBeenBanned notification, CancellationToken cancellationToken)
    {
        await _emailService.SendMailAsync
                            (notification.Email,
                            "Ban Notification",
                            "EmailAccountBanned.html", new Dictionary<string, string> {
                    { "ToEmail", notification.Email},
                    {"BanReason", notification.BanReason }
                            });
    }

    public async Task Handle(DomainEvent.AccountHasBeenUnbanned notification, CancellationToken cancellationToken)
    {
        await _emailService.SendMailAsync
                            (notification.Email,
                            "Unban Notification",
                            "EmailAccountUnbanned.html", new Dictionary<string, string> {
                    { "ToEmail", notification.Email},
                            });
    }
}
