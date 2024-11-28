using Microsoft.Extensions.Options;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Services.Orders;
using Neighbor.Contract.Settings;

namespace Neighbor.Application.UseCases.V2.Events;

public sealed class SendEmailWhenUserOrderProductEventHandler
    : IDomainEventHandler<DomainEvent.NotiLessorOrderSuccess>,
    IDomainEventHandler<DomainEvent.NotiUserOrderSuccess>
{

    private readonly IEmailService _emailService;
    private readonly ClientSetting _clientSetting;

    public SendEmailWhenUserOrderProductEventHandler(IEmailService emailService,
        IOptions<ClientSetting> clientConfig)
    {
        _emailService = emailService;
        _clientSetting = clientConfig.Value;
    }

    public async Task Handle(DomainEvent.NotiLessorOrderSuccess notification, CancellationToken cancellationToken)
    {
        await _emailService.SendMailAsync
                            (notification.Email,
                            "Order Notification",
                            "EmailNotiLessorOrder.html", new Dictionary<string, string> {
                            {"ToEmail", notification.Email},
                            {"ProductName", notification.ProductName },
                            {"DeliveryAddress", notification.DeliveryAddress },
                            {"MeetingDate", notification.MeetingDate.ToString("dd/MM/yyyy") },
                            {"UserEmail", notification.UserEmail }
                            });
    }

    public async Task Handle(DomainEvent.NotiUserOrderSuccess notification, CancellationToken cancellationToken)
    {
        await _emailService.SendMailAsync
                            (notification.Email,
                            "Order Notification",
                            "EmailNotiUserOrder.html", new Dictionary<string, string> {
                            {"ToEmail", notification.Email},
                            {"ProductName", notification.ProductName },
                            {"DeliveryAddress", notification.DeliveryAddress },
                            {"MeetingDate", notification.MeetingDate.ToString("dd/MM/yyyy") }
                            });
    }
}
