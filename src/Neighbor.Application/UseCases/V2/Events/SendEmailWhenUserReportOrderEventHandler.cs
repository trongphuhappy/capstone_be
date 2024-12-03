using Microsoft.Extensions.Options;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Services.Orders;
using Neighbor.Contract.Settings;

namespace Neighbor.Application.UseCases.V2.Events;

public sealed class SendEmailWhenUserReportOrderEventHandler
    : IDomainEventHandler<DomainEvent.NotiLessorAboutUserReportedOrderSuccess>
{

    private readonly IEmailService _emailService;
    private readonly ClientSetting _clientSetting;

    public SendEmailWhenUserReportOrderEventHandler(IEmailService emailService,
        IOptions<ClientSetting> clientConfig)
    {
        _emailService = emailService;
        _clientSetting = clientConfig.Value;
    }

    public async Task Handle(DomainEvent.NotiLessorAboutUserReportedOrderSuccess notification, CancellationToken cancellationToken)
    {
        await _emailService.SendMailAsync(notification.Email,
                                         "Order Notification",
                                         "EmailNotiLessorAboutUserReportedOrder.html", new Dictionary<string, string> {
                                         {"ToEmail", notification.Email},
                                         {"ProductName", notification.ProductName },
                                         {"UserReport", notification.UserReport }
                                         });
    }
}
