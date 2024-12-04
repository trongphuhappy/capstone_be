using MediatR;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Contract.Enumarations.Order;
using Neighbor.Contract.Services.Orders;
using Neighbor.Domain.Abstraction.EntitiyFramework;
using Neighbor.Domain.Exceptions;

namespace Neighbor.Application.UseCases.V2.Commands.Orders;
public sealed class AdminConfirmOrderCommandHandler : ICommandHandler<Command.AdminConfirmOrderCommand>
{
    private readonly IEFUnitOfWork _efUnitOfWork;
    private readonly IPublisher _publisher;

    public AdminConfirmOrderCommandHandler(IEFUnitOfWork efUnitOfWork, IPublisher publisher)
    {
        _efUnitOfWork = efUnitOfWork;
        _publisher = publisher;
    }

    public async Task<Result> Handle(Command.AdminConfirmOrderCommand request, CancellationToken cancellationToken)
    {
        var orderFound = await _efUnitOfWork.OrderRepository.FindByIdAsync(request.OrderId);
        if (orderFound == null)
        {
            throw new OrderException.OrderNotFoundException();
        }
        if (orderFound.OrderReportStatus == OrderReportStatusType.NotConflict)
        {
            throw new OrderException.OrderDoesNotConflictException();
        }
        if (orderFound.OrderReportStatus != OrderReportStatusType.PendingConflict)
        {
            throw new OrderException.OrderHaveAlreadyConfirmedException();
        }
        if (request.IsApproved == false && request.RejectReason == null)
        {
            throw new OrderException.OrderRejectWithoutReasonException();
        }

        string rejectReason = !request.IsApproved ? request.RejectReason : null;
        //Check IsApproved to assign value to order status
        var orderReportStatus = request.IsApproved ? OrderReportStatusType.ApprovedReport : OrderReportStatusType.RejectedReport;
        orderFound.UpdateConfirmOrderByAdmin(orderReportStatus, rejectReason);
        _efUnitOfWork.OrderRepository.Update(orderFound);
        await _efUnitOfWork.SaveChangesAsync(cancellationToken);
        if (request.IsApproved)
        {
            //Refund money to User if User Report has been approved by Admin
            var wallet = await _efUnitOfWork.WalletRepository.GetWalletByLessorId(orderFound.Product.Lessor.Id);
            int orderValue = (int)((orderFound.ReturnTime - orderFound.RentTime).TotalDays) * orderFound.Product.Price;
            wallet.WithdrawMoney((int)(orderValue * 0.3), $"Admin refund order {request.OrderId}");
            _efUnitOfWork.WalletRepository.Update(wallet);

            //Send approved email
            //Send email to Lessor
            await Task.WhenAll(
               _publisher.Publish(new DomainEvent.NotiLessorAboutAdminApprovedReportedOrderSuccess(request.OrderId, orderFound.Product.Lessor.Account.Email, orderFound.Product.Name), cancellationToken)
           );
            //Send email to User
            await Task.WhenAll(
               _publisher.Publish(new DomainEvent.NotiUserAboutAdminApprovedReportedOrderSuccess(request.OrderId, orderFound.Account.Email, orderFound.Product.Name), cancellationToken)
           );
        }
        else
        {
            //Send rejected email
            //Send email to Lessor
            await Task.WhenAll(
               _publisher.Publish(new DomainEvent.NotiLessorAboutAdminRejectedReportedOrderSuccess(request.OrderId, orderFound.Product.Lessor.Account.Email, orderFound.Product.Name, request.RejectReason), cancellationToken)
           );
            //Send email to User
            await Task.WhenAll(
               _publisher.Publish(new DomainEvent.NotiUserAboutAdminRejectedReportedOrderSuccess(request.OrderId, orderFound.Account.Email, orderFound.Product.Name, request.RejectReason), cancellationToken)
           );
        }
        //Return result
        return Result.Success(new Success(MessagesList.OrderConfirmSuccessfully.GetMessage().Code, MessagesList.OrderConfirmSuccessfully.GetMessage().Message));
    }
}
