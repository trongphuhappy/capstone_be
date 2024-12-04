using MediatR;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Contract.Enumarations.Order;
using Neighbor.Contract.Services.Orders;
using Neighbor.Domain.Abstraction.EntitiyFramework;
using Neighbor.Domain.Exceptions;

namespace Neighbor.Application.UseCases.V2.Commands.Orders;
public sealed class UserReportOrderCommandHandler : ICommandHandler<Command.UserReportOrderCommand>
{
    private readonly IEFUnitOfWork _efUnitOfWork;
    private readonly IPublisher _publisher;

    public UserReportOrderCommandHandler(IEFUnitOfWork efUnitOfWork, IPublisher publisher)
    {
        _efUnitOfWork = efUnitOfWork;
        _publisher = publisher;
    }

    public async Task<Result> Handle(Command.UserReportOrderCommand request, CancellationToken cancellationToken)
    {
        var orderFound = await _efUnitOfWork.OrderRepository.FindByIdAsync(request.OrderId);
        if (orderFound == null)
        {
            throw new OrderException.OrderNotFoundException();
        }
        if (orderFound.Account.Id != request.AccountId)
        {
            throw new OrderException.OrderNotBelongToUserException();
        }
        if (orderFound.OrderStatus != OrderStatusType.CompletedRented && orderFound.OrderStatus != OrderStatusType.RejectionValidated && orderFound.OrderStatus != OrderStatusType.RejectionInvalidated)
        {
            throw new OrderException.OrderLessorHasNotConfirmException();
        }
        orderFound.UpdateReportOrder(request.UserReport);
        _efUnitOfWork.OrderRepository.Update(orderFound);
        await _efUnitOfWork.SaveChangesAsync(cancellationToken);
        //Send Mail to Lessor
        await Task.WhenAll(
               _publisher.Publish(new DomainEvent.NotiLessorAboutUserReportedOrderSuccess(request.OrderId, orderFound.Product.Lessor.Account.Email, orderFound.Product.Name, request.UserReport), cancellationToken)
           );
        //Return result
        return Result.Success(new Success(MessagesList.OrderReportSuccessfully.GetMessage().Code, MessagesList.OrderReportSuccessfully.GetMessage().Message));
    }
}
