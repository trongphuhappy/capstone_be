using MediatR;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Contract.Enumarations.Order;
using Neighbor.Contract.Enumarations.Product;
using Neighbor.Contract.Services.Orders;
using Neighbor.Domain.Abstraction.EntitiyFramework;
using Neighbor.Domain.Entities;
using Neighbor.Domain.Exceptions;

namespace Neighbor.Application.UseCases.V2.Commands.Orders;

public sealed class CreateOrderBankingCommandHandler : ICommandHandler<Command.CreateOrderBankingCommand>
{
    private readonly IEFUnitOfWork _efUnitOfWork;
    private readonly IPublisher _publisher;

    public CreateOrderBankingCommandHandler(
        IEFUnitOfWork efUnitOfWork, IPublisher publisher)
    {
        _efUnitOfWork = efUnitOfWork;
        _publisher = publisher;
    }

    public async Task<Result> Handle(Command.CreateOrderBankingCommand request, CancellationToken cancellationToken)
    {
        //CreateOrderBankingCommandHandler
        var accountFound = await _efUnitOfWork.AccountRepository.FindByIdAsync(request.AccountId);
        if (accountFound == null)
        {
            throw new AccountException.AccountNotFoundException();
        }
        var productFound = await _efUnitOfWork.ProductRepository.FindByIdAsync(request.ProductId);
        if (productFound == null)
        {
            throw new ProductException.ProductNotFoundException();
        }
        if(productFound.Lessor.AccountId == request.AccountId)
        {
            throw new OrderException.ProductBelongsToUserException();
        }
        var isUserOrderProduct = await _efUnitOfWork.OrderRepository.AnyAsync(order => order.ProductId == request.ProductId && order.AccountId == request.AccountId && order.OrderStatus != OrderStatusType.RejectionValidated && order.OrderStatus != OrderStatusType.RejectionInvalidated);
        if(isUserOrderProduct)
        {
            throw new OrderException.ProductAlreadyOrderedByUserException();
        }
        if(productFound.StatusType != StatusType.Available)
        {
            throw new OrderException.ProductOrderedByAnotherUserException();
        }
        if(productFound.ConfirmStatus != ConfirmStatus.Approved)
        {
            throw new OrderException.ProductNotApprovedByAdminException();
        }

        //SuccessOrderBankingCommandHandler
        long orderId = 3;
        var orderCreated = Order.CreateOrder(request.AccountId, request.ProductId, request.RentTime, request.ReturnTime, productFound.Lessor.WareHouseAddress, productFound.Price, orderId);
        _efUnitOfWork.OrderRepository.Add(orderCreated);
        await _efUnitOfWork.SaveChangesAsync(cancellationToken);
        var productSuccessFound = await _efUnitOfWork.ProductRepository.FindByIdAsync(orderCreated.ProductId.Value);
        if (productSuccessFound == null)
        {
            throw new ProductException.ProductNotFoundException();
        }
        productSuccessFound.UpdateStatusType(StatusType.Not_Available);
        _efUnitOfWork.ProductRepository.Update(productSuccessFound);
        await _efUnitOfWork.SaveChangesAsync(cancellationToken);
        //Send success order email for Lessor
        await Task.WhenAll(
           _publisher.Publish(new DomainEvent.NotiLessorOrderSuccess(orderCreated.Id, productFound.Lessor.Account.Email, productFound.Name, productFound.Lessor.WareHouseAddress, request.RentTime, accountFound.Email), cancellationToken)
       );
        //Send success order email for User
        await Task.WhenAll(
           _publisher.Publish(new DomainEvent.NotiUserOrderSuccess(orderCreated.Id, accountFound.Email, productFound.Name, productFound.Lessor.WareHouseAddress, request.RentTime), cancellationToken)
       );
        return Result.Success();
    }
}
