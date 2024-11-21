using Microsoft.Extensions.Options;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Services.Products;
using Neighbor.Contract.Settings;

namespace Neighbor.Application.UseCases.V2.Events;

public sealed class SendEmailWhenAdminConfirmProductEventHandler
    : IDomainEventHandler<DomainEvent.ProductHasBeenApproved>,
    IDomainEventHandler<DomainEvent.ProductHasBeenRejected>
{

    private readonly IEmailService _emailService;
    private readonly ClientSetting _clientSetting;

    public SendEmailWhenAdminConfirmProductEventHandler(IEmailService emailService,
        IOptions<ClientSetting> clientConfig)
    {
        _emailService = emailService;
        _clientSetting = clientConfig.Value;
    }

    public async Task Handle(DomainEvent.ProductHasBeenApproved notification, CancellationToken cancellationToken)
    {
        await _emailService.SendMailAsync
                    (notification.Email,
                    "Confirm Product",
                    "EmailProductApproved.html", new Dictionary<string, string> {
                    { "ToEmail", notification.Email},
                    {"ProductName", notification.ProductName }
                    });
    }

    public async Task Handle(DomainEvent.ProductHasBeenRejected notification, CancellationToken cancellationToken)
    {
        await _emailService.SendMailAsync
                    (notification.Email,
                    "Confirm Product",
                    "EmailProductRejected.html", new Dictionary<string, string> {
                    { "ToEmail", notification.Email},
                    {"ProductName", notification.ProductName },
                    {"RejectReason", notification.RejectReason }
                    });
    }
}
