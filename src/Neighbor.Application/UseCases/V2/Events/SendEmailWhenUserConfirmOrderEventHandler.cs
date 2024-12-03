using Microsoft.Extensions.Options;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Services.Orders;
using Neighbor.Contract.Settings;

namespace Neighbor.Application.UseCases.V2.Events;

public sealed class SendEmailWhenUserConfirmOrderEventHandler
    : IDomainEventHandler<DomainEvent.NotiLessorAboutUserApprovedOrderSuccess>,
    IDomainEventHandler<DomainEvent.NotiLessorAboutUserRejectedOrderSuccess>
{

    private readonly IEmailService _emailService;
    private readonly ClientSetting _clientSetting;

    public SendEmailWhenUserConfirmOrderEventHandler(IEmailService emailService,
        IOptions<ClientSetting> clientConfig)
    {
        _emailService = emailService;
        _clientSetting = clientConfig.Value;
    }

    public async Task Handle(DomainEvent.NotiLessorAboutUserApprovedOrderSuccess notification, CancellationToken cancellationToken)
    {
        await _emailService.SendMailAsync
                                    (notification.Email,
                                    "Order Notification",
                                    "EmailNotiLessorAboutUserApprovedOrder.html", new Dictionary<string, string> {
                                    {"ToEmail", notification.Email},
                                    {"ProductName", notification.ProductName },
                                    });
    }

    public async Task Handle(DomainEvent.NotiLessorAboutUserRejectedOrderSuccess notification, CancellationToken cancellationToken)
    {
        await _emailService.SendMailAsync
                                    (notification.Email,
                                    "Order Notification",
                                    "EmailNotiLessorAboutUserRejectedOrder.html", new Dictionary<string, string> {
                                    {"ToEmail", notification.Email},
                                    {"ProductName", notification.ProductName },
                                    {"RejectReason", notification.RejectReason }
                                    });
    }
}
