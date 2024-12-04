using Microsoft.Extensions.Options;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Services.Orders;
using Neighbor.Contract.Settings;

namespace Neighbor.Application.UseCases.V2.Events;

public sealed class SendEmailWhenAdminConfirmOrderEventHandler
    : IDomainEventHandler<DomainEvent.NotiLessorAboutAdminApprovedReportedOrderSuccess>,
      IDomainEventHandler<DomainEvent.NotiLessorAboutAdminRejectedReportedOrderSuccess>,
      IDomainEventHandler<DomainEvent.NotiUserAboutAdminApprovedReportedOrderSuccess>,
      IDomainEventHandler<DomainEvent.NotiUserAboutAdminRejectedReportedOrderSuccess>
{
    private readonly IEmailService _emailService;
    private readonly ClientSetting _clientSetting;

    public SendEmailWhenAdminConfirmOrderEventHandler(IEmailService emailService,
        IOptions<ClientSetting> clientConfig)
    {
        _emailService = emailService;
        _clientSetting = clientConfig.Value;
    }

    public async Task Handle(DomainEvent.NotiLessorAboutAdminApprovedReportedOrderSuccess notification, CancellationToken cancellationToken)
    {
        await _emailService.SendMailAsync(notification.Email,
                                         "Order Notification",
                                         "EmailNotiLessorAboutAdminApprovedReportedOrder.html", new Dictionary<string, string> {
                                         {"ToEmail", notification.Email},
                                         {"ProductName", notification.ProductName },
                                         });
    }

    public async Task Handle(DomainEvent.NotiLessorAboutAdminRejectedReportedOrderSuccess notification, CancellationToken cancellationToken)
    {
        await _emailService.SendMailAsync(notification.Email,
                                         "Order Notification",
                                         "EmailNotiLessorAboutAdminRejectedReportedOrderSuccess.html", new Dictionary<string, string> {
                                         {"ToEmail", notification.Email},
                                         {"ProductName", notification.ProductName },
                                         {"RejectReason", notification.RejectReason }
                                         });
    }

    public async Task Handle(DomainEvent.NotiUserAboutAdminApprovedReportedOrderSuccess notification, CancellationToken cancellationToken)
    {
        await _emailService.SendMailAsync(notification.Email,
                                         "Order Notification",
                                         "EmailNotiUserAboutAdminApprovedReportedOrderSuccess.html", new Dictionary<string, string> {
                                         {"ToEmail", notification.Email},
                                         {"ProductName", notification.ProductName },
                                         });
    }

    public async Task Handle(DomainEvent.NotiUserAboutAdminRejectedReportedOrderSuccess notification, CancellationToken cancellationToken)
    {
        await _emailService.SendMailAsync(notification.Email,
                                         "Order Notification",
                                         "EmailNotiUserAboutAdminRejectedReportedOrderSuccess.html", new Dictionary<string, string> {
                                         {"ToEmail", notification.Email},
                                         {"ProductName", notification.ProductName },
                                         {"RejectReason", notification.RejectReason }
                                         });
    }
}
