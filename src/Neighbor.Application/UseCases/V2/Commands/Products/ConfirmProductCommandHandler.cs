﻿using MediatR;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Contract.Enumarations.Product;
using Neighbor.Contract.Services.Products;
using Neighbor.Domain.Abstraction.EntitiyFramework;
using Neighbor.Domain.Exceptions;

namespace Neighbor.Application.UseCases.V2.Commands.Products;

public sealed class ConfirmProductCommandHandler : ICommandHandler<Command.ConfirmProductCommand>
{
    private readonly IEFUnitOfWork _efUnitOfWork;
    private readonly IPublisher _publisher;

    public ConfirmProductCommandHandler(
        IEFUnitOfWork efUnitOfWork,
        IPublisher publisher)
    {
        _efUnitOfWork = efUnitOfWork;
        _publisher = publisher;
    }

    public async Task<Result> Handle(Command.ConfirmProductCommand request, CancellationToken cancellationToken)
    {
        //Check if Product exists
        var productFound = await _efUnitOfWork.ProductRepository.FindByIdAsync(request.ProductId);
        if (productFound == null)
        {
            throw new ProductException.ProductNotFoundException();
        } 
        if(productFound.ConfirmStatus == ConfirmStatus.Approved)
        {
            throw new ProductException.ProductHasAlreadyApprovedException();
        }else if(productFound.ConfirmStatus == ConfirmStatus.Rejected)
        {
            throw new ProductException.ProductHasAlreadyRejectedException();
        }
        //Check if reject then must have reason
        if (request.ConfirmStatus == ConfirmStatus.Rejected && request.RejectReason == null)
        {
            throw new ProductException.ProductRejectNoReasonException();
        }
        string rejectReason = request.ConfirmStatus == ConfirmStatus.Rejected ? request.RejectReason : null;
        //Update Confirm Status to DB
        productFound.UpdateProduct(productFound.Name, productFound.Policies, productFound.Description, productFound.Price, productFound.Value, productFound.MaximumRentDays, rejectReason, productFound.StatusType, request.ConfirmStatus);
        _efUnitOfWork.ProductRepository.Update(productFound);
        await _efUnitOfWork.SaveChangesAsync(cancellationToken);
        //Send email
        if(request.ConfirmStatus == ConfirmStatus.Approved)
        {
            //Send approved email
            await Task.WhenAll(
               _publisher.Publish(new DomainEvent.ProductHasBeenApproved(request.ProductId, productFound.Lessor.Account.Email, productFound.Name), cancellationToken)
           );
        }
        else
        {
            //Send rejected email
            await Task.WhenAll(
               _publisher.Publish(new DomainEvent.ProductHasBeenRejected(request.ProductId, productFound.Lessor.Account.Email, productFound.Name, request.RejectReason), cancellationToken)
           );
        }
        //Return result
        return Result.Success(new Success(MessagesList.ProductConfirmSuccess.GetMessage().Code, MessagesList.ProductConfirmSuccess.GetMessage().Message));
    }
}
