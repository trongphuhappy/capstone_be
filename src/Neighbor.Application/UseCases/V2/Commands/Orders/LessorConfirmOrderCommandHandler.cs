using MediatR;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Contract.Enumarations.Order;
using Neighbor.Contract.Enumarations.Product;
using Neighbor.Contract.Services.Orders;
using Neighbor.Domain.Abstraction.EntitiyFramework;
using Neighbor.Domain.Entities;
using Neighbor.Domain.Exceptions;

namespace Neighbor.Application.UseCases.V2.Commands.Orders;
public sealed class LessorConfirmOrderCommandHandler : ICommandHandler<Command.LessorConfirmOrderCommand>
{
    private readonly IEFUnitOfWork _efUnitOfWork;
    private readonly IPublisher _publisher;

    public LessorConfirmOrderCommandHandler(IEFUnitOfWork efUnitOfWork, IPublisher publisher)
    {
        _efUnitOfWork = efUnitOfWork;
        _publisher = publisher;
    }

    public async Task<Result> Handle(Command.LessorConfirmOrderCommand request, CancellationToken cancellationToken)
    {
        var orderFound = await _efUnitOfWork.OrderRepository.FindByIdAsync(request.OrderId);
        if (orderFound == null)
        {
            throw new OrderException.OrderNotFoundException();
        }
        if (orderFound.Product.Lessor.Account.Id != request.AccountId)
        {
            throw new OrderException.OrderNotBelongToUserException();
        }
        if (orderFound.OrderStatus == OrderStatusType.Pending)
        {
            throw new OrderException.OrderUserHasNotConfirmException();
        }
        if (orderFound.OrderStatus != OrderStatusType.UserApproved && orderFound.OrderStatus != OrderStatusType.UserRejected)
        {
            throw new OrderException.OrderHaveAlreadyConfirmedException();
        }
        if (request.IsApproved == false && request.RejectReason == null)
        {
            throw new OrderException.OrderRejectWithoutReasonException();
        }
        string rejectReason = !request.IsApproved ? request.RejectReason : null;
        //Refund money to User if User Rejected Order and Lessor Approved this Rejected
        if (request.IsApproved && orderFound.OrderStatus == OrderStatusType.UserRejected)
        {
            var wallet = await _efUnitOfWork.WalletRepository.GetWalletByLessorId(orderFound.Product.Lessor.Id);
            long orderValue = (long)((orderFound.ReturnTime - orderFound.RentTime).TotalDays) * orderFound.Product.Price;
            wallet.WithdrawMoney((long)(orderValue * 0.3), $"Lessor with accountId {request.AccountId} refund order {request.OrderId}");
            _efUnitOfWork.WalletRepository.Update(wallet);
        }
        //Check IsApproved to assign value to order status
        //If IsApproved then Check if User has Approved or Rejected this Order before
        var orderStatus = request.IsApproved
            ?
            (orderFound.OrderStatus == OrderStatusType.UserApproved ? OrderStatusType.CompletedRented : OrderStatusType.RejectionValidated) 
            : OrderStatusType.RejectionInvalidated;
        orderFound.UpdateConfirmOrderByLessor(orderStatus, rejectReason);
        _efUnitOfWork.OrderRepository.Update(orderFound);
        await _efUnitOfWork.SaveChangesAsync(cancellationToken);
        //Check if Order Not Completed then Update Product Available for rented
        if(orderStatus != OrderStatusType.CompletedRented)
        {
            var productFound = await _efUnitOfWork.ProductRepository.FindByIdAsync(orderFound.Product.Id);
            productFound.UpdateStatusType(StatusType.Available);
            _efUnitOfWork.ProductRepository.Update(productFound);
            await _efUnitOfWork.SaveChangesAsync(cancellationToken);
        }
        
        //Send email
        if (request.IsApproved)
        {
            //Send approved email
            await Task.WhenAll(
               _publisher.Publish(new DomainEvent.NotiUserAboutLessorApprovedOrderSuccess(request.OrderId, orderFound.Account.Email, orderFound.Product.Name, orderStatus), cancellationToken)
           );
        }
        else
        {
            //Send rejected email
            await Task.WhenAll(
               _publisher.Publish(new DomainEvent.NotiUserAboutLessorRejectedOrderSuccess(request.OrderId, orderFound.Account.Email, orderFound.Product.Name, request.RejectReason), cancellationToken)
           );
        }
        //Return result
        return Result.Success(new Success(MessagesList.OrderConfirmSuccessfully.GetMessage().Code, MessagesList.OrderConfirmSuccessfully.GetMessage().Message));
    }
}