using MediatR;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Contract.Enumarations.Order;
using Neighbor.Contract.Enumarations.Product;
using Neighbor.Contract.Services.Orders;
using Neighbor.Domain.Abstraction.EntitiyFramework;
using Neighbor.Domain.Exceptions;

namespace Neighbor.Application.UseCases.V2.Commands.Orders;
public sealed class UserConfirmOrderCommandHandler : ICommandHandler<Command.UserConfirmOrderCommand>
{
    private readonly IEFUnitOfWork _efUnitOfWork;
    private readonly IPublisher _publisher;

    public UserConfirmOrderCommandHandler(IEFUnitOfWork efUnitOfWork, IPublisher publisher)
    {
        _efUnitOfWork = efUnitOfWork;
        _publisher = publisher;
    }

    public async Task<Result> Handle(Command.UserConfirmOrderCommand request, CancellationToken cancellationToken)
    {
        var orderFound = await _efUnitOfWork.OrderRepository.FindByIdAsync(request.OrderId);
        if(orderFound == null)
        {
            throw new OrderException.OrderNotFoundException();
        }
        if(orderFound.Account.Id != request.AccountId)
        {
            throw new OrderException.OrderNotBelongToUserException();
        }
        if(orderFound.OrderStatus != OrderStatusType.Pending)
        {
            throw new OrderException.OrderHaveAlreadyConfirmedException();
        }
        if(request.IsApproved == false && request.RejectReason == null)
        {
            throw new OrderException.OrderRejectWithoutReasonException();
        }
        string rejectReason = !request.IsApproved ? request.RejectReason : null;
        //Check IsApproved to assign value to order status
        var orderStatus = request.IsApproved ? OrderStatusType.UserApproved : OrderStatusType.UserRejected;
        orderFound.UpdateConfirmOrderByUser(orderStatus, rejectReason);
        _efUnitOfWork.OrderRepository.Update(orderFound);
        await _efUnitOfWork.SaveChangesAsync(cancellationToken);
        //Send email
        if (request.IsApproved)
        {
            //Send approved email
            await Task.WhenAll(
               _publisher.Publish(new DomainEvent.NotiLessorAboutUserApprovedOrderSuccess(request.OrderId, orderFound.Product.Lessor.Account.Email, orderFound.Product.Name), cancellationToken)
           );
        }
        else
        {
            //Send rejected email
            await Task.WhenAll(
               _publisher.Publish(new DomainEvent.NotiLessorAboutUserRejectedOrderSuccess(request.OrderId, orderFound.Product.Lessor.Account.Email, orderFound.Product.Name, request.RejectReason), cancellationToken)
           );
        }
        //Return result
        return Result.Success(new Success(MessagesList.OrderConfirmSuccessfully.GetMessage().Code, MessagesList.OrderConfirmSuccessfully.GetMessage().Message));
    }
}
