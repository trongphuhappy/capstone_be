using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Contract.Services.Products;
using Neighbor.Domain.Abstraction.EntitiyFramework;
using Neighbor.Domain.Entities;
using Neighbor.Domain.Exceptions;

namespace Neighbor.Application.UseCases.V2.Commands.Products;

public sealed class AddToWishlistCommandHandler : ICommandHandler<Command.AddToWishlistCommand>
{
    private readonly IEFUnitOfWork _efUnitOfWork;

    public AddToWishlistCommandHandler(
        IEFUnitOfWork efUnitOfWork)
    {
        _efUnitOfWork = efUnitOfWork;
    }

    public async Task<Result> Handle(Command.AddToWishlistCommand request, CancellationToken cancellationToken)
    {
        //Check if Account exist
        var accountFound = await _efUnitOfWork.AccountRepository.FindByIdAsync(request.AccountId);
        if (accountFound == null)
        {
            throw new AccountException.AccountNotFoundException();
        }
        //Check if Product exist
        var productFound = await _efUnitOfWork.ProductRepository.FindByIdAsync(request.ProductId);
        if (productFound == null)
        {
            throw new ProductException.ProductNotFoundException();
        }
        //Check if User is a Lessor then Check if product belongs to User
        if (productFound.Lessor.AccountId == request.AccountId)
        {
            throw new WishlistException.ProductBelongsToUserException();
        }
        //Check if Product is on Account's Wishlist or not
        var isProductInWishlist = await _efUnitOfWork.WishlistRepository.FindSingleAsync(x => x.ProductId == request.ProductId && x.AccountId == request.AccountId);

        //Add to Wishlist
        if (isProductInWishlist == null)
        {
            var wishlist = Wishlist.AddProductToWishlist(request.AccountId, request.ProductId);
            _efUnitOfWork.WishlistRepository.Add(wishlist);
            await _efUnitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success(new Success(MessagesList.ProductAddToWishlistSuccess.GetMessage().Code, MessagesList.ProductAddToWishlistSuccess.GetMessage().Message));
        }
        //Remove from Wishlist
        else
        {
            var wishlist = await _efUnitOfWork.WishlistRepository.FindSingleAsync(wishlist => wishlist.AccountId == request.AccountId && wishlist.ProductId == request.ProductId);
            _efUnitOfWork.WishlistRepository.Remove(wishlist);
            await _efUnitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success(new Success(MessagesList.ProductRemoveFromWishlistlistSuccess.GetMessage().Code, MessagesList.ProductRemoveFromWishlistlistSuccess.GetMessage().Message));
        }


    }
}
