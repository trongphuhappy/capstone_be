using MediatR;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.DTOs.ProductDTOs;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Contract.Enumarations.Product;
using Neighbor.Contract.Services.Admins;
using Neighbor.Domain.Abstraction.EntitiyFramework;
using Neighbor.Domain.Exceptions;

namespace Neighbor.Application.UseCases.V2.Commands.Admins;

public sealed class HandleUserCommandHandler : ICommandHandler<Command.HandleUserCommand>
{
    private readonly IEFUnitOfWork _efUnitOfWork;
    private readonly IPublisher _publisher;
    private readonly IMediaService _mediaService;
    
    public HandleUserCommandHandler(
        IEFUnitOfWork efUnitOfWork,
        IPublisher publisher, IMediaService mediaService)
    {
        _efUnitOfWork = efUnitOfWork;
        _publisher = publisher;
        _mediaService = mediaService;
    }

    public async Task<Result> Handle(Command.HandleUserCommand request, CancellationToken cancellationToken)
    {
        //Check if Account exists
        var accountFound = await _efUnitOfWork.AccountRepository.FindByIdAsync(request.AccountId);
        if (accountFound == null)
        {
            throw new AccountException.AccountNotFoundException();
        }
        if (accountFound.IsDeleted == true && request.IsDeleted == true)
        {
            throw new AccountException.AccountHasAlreadyBannedException();
        }
        else if (accountFound.IsDeleted == false && request.IsDeleted == false)
        {
            throw new AccountException.AccountHasAlreadyUnbannedException();
        }
        //Check if reject then must have reason
        if (request.IsDeleted == true && request.BanReason == null)
        {
            throw new AdminException.BanWithNoReasonException();
        }
        //Update Is Deleted to DB
        accountFound.UpdateIsDeletedForAccount(request.IsDeleted);
        _efUnitOfWork.AccountRepository.Update(accountFound);
        await _efUnitOfWork.SaveChangesAsync(cancellationToken);
        //Send email
        if (request.IsDeleted == true)
        {
            //Send ban email
            await Task.WhenAll(
               _publisher.Publish(new DomainEvent.AccountHasBeenBanned(request.AccountId, accountFound.Email, request.BanReason), cancellationToken)
           );
            //Return result
            return Result.Success(new Success(MessagesList.AdminBanUserSuccess.GetMessage().Code, MessagesList.AdminBanUserSuccess.GetMessage().Message));
        }
        //Send unban email
        await Task.WhenAll(
           _publisher.Publish(new DomainEvent.AccountHasBeenUnbanned(request.AccountId, accountFound.Email), cancellationToken)
       );
        //Return result
        return Result.Success(new Success(MessagesList.AdminUnbanUserSuccess.GetMessage().Code, MessagesList.AdminUnbanUserSuccess.GetMessage().Message));

    }
}
