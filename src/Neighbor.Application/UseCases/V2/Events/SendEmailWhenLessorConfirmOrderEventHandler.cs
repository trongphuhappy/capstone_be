using Microsoft.Extensions.Options;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Enumarations.Order;
using Neighbor.Contract.Services.Orders;
using Neighbor.Contract.Settings;

namespace Neighbor.Application.UseCases.V2.Events;

public sealed class SendEmailWhenLessorConfirmOrderEventHandler
    : IDomainEventHandler<DomainEvent.NotiUserAboutLessorApprovedOrderSuccess>,
    IDomainEventHandler<DomainEvent.NotiUserAboutLessorRejectedOrderSuccess>
{

    private readonly IEmailService _emailService;
    private readonly ClientSetting _clientSetting;

    public SendEmailWhenLessorConfirmOrderEventHandler(IEmailService emailService,
        IOptions<ClientSetting> clientConfig)
    {
        _emailService = emailService;
        _clientSetting = clientConfig.Value;
    }

    public async Task Handle(DomainEvent.NotiUserAboutLessorApprovedOrderSuccess notification, CancellationToken cancellationToken)
    {
        if(notification.OrderStatus == OrderStatusType.CompletedRented)
        {
            await _emailService.SendMailAsync
            (notification.Email,
            "Order Notification",
            "EmailNotiUserAboutLessorApprovedCompletedOrder.html", new Dictionary<string, string> {
            {"ToEmail", notification.Email},
            {"ProductName", notification.ProductName },
            });
            return;
        }
        if (notification.OrderStatus == OrderStatusType.RejectionValidated)
        {
            await _emailService.SendMailAsync
            (notification.Email,
            "Order Notification",
            "EmailNotiUserAboutLessorApprovedRejectReasonOrder.html", new Dictionary<string, string> {
            {"ToEmail", notification.Email},
            {"ProductName", notification.ProductName },
            });
            return;
        }
    }

    public async Task Handle(DomainEvent.NotiUserAboutLessorRejectedOrderSuccess notification, CancellationToken cancellationToken)
    {
        await _emailService.SendMailAsync
                                            (notification.Email,
                                            "Order Notification",
                                            "EmailNotiUserAboutLessorRejectedOrder.html", new Dictionary<string, string> {
                                            {"ToEmail", notification.Email},
                                            {"ProductName", notification.ProductName },
                                            {"RejectReason", notification.RejectReason },
                                            });
    }
}
