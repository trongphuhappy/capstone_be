using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Neighbor.Application.UseCases.V2.Commands.Products;
using Neighbor.Domain.Abstraction.EntitiyFramework;
using Neighbor.Domain.Entities;
using Neighbor.Domain.Exceptions;
using System.Linq.Expressions;
using Neighbor.Contract.Services.Products;
using Microsoft.Identity.Client;
namespace Neighbor.Application.UnitTests.UseCases.V2.Commands.Products;

public class AddToWishlistCommandHandlerTests
{
    private readonly Mock<IEFUnitOfWork> _mockEFUnitOfWork;
    private readonly AddToWishlistCommandHandler _handler;

    public AddToWishlistCommandHandlerTests()
    {
        _mockEFUnitOfWork = new Mock<IEFUnitOfWork>();
        _handler = new AddToWishlistCommandHandler(_mockEFUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_AccountNotFound_ThrowsAccountNotFoundException()
    {
        // Arrange
        var command = new Command.AddToWishlistCommand 
        ( 
            AccountId: Guid.NewGuid(), 
            ProductId: Guid.NewGuid() 
        );

        // Mock the AccountRepository to simulate account is not found.
        _mockEFUnitOfWork.Setup(x => x.AccountRepository.FindByIdAsync(
            command.AccountId,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Account, object>>[]>()
        )).ReturnsAsync((Account?) null);

        // Act & Assert
        await Assert.ThrowsAsync<AccountException.AccountNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ProductNotFound_ThrowsProductNotFoundException()
    {
        // Arrange
        var command = new Command.AddToWishlistCommand
                (
                    AccountId: Guid.NewGuid(),
                    ProductId: Guid.NewGuid()
                );

        // Mock the AccountRepository to simulate account is existed.
        _mockEFUnitOfWork.Setup(x => x.AccountRepository.FindByIdAsync(
            command.AccountId,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Account, object>>[]>()
        )).ReturnsAsync(new Account());

        // Mock the ProductRepository to simulate product is not found.
        _mockEFUnitOfWork.Setup(x => x.ProductRepository.FindByIdAsync(
            command.AccountId,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Product, object>>[]>()
        )).ReturnsAsync((Product?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ProductException.ProductNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
        public async Task Handle_ProductBelongsToUser_ThrowsProductBelongsToUserException()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        
        var command = new Command.AddToWishlistCommand
        (
            AccountId: accountId,
            ProductId: Guid.NewGuid()
        );

        // Mock the AccountRepository to simulate account is existed.
        _mockEFUnitOfWork.Setup(x => x.AccountRepository.FindByIdAsync(
            command.AccountId,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Account, object>>[]>()
        )).ReturnsAsync(Account.CreateAccountForAddToWishlistCommandHandlerTest(accountId));

        // Mock the ProductRepository to simulate product is belonged to user.
        _mockEFUnitOfWork.Setup(x => x.ProductRepository.FindByIdAsync(
            command.ProductId,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Product, object>>[]>()
        )).ReturnsAsync(Product.CreateProductForAddToWishlistCommandHandlerTest(Lessor.CreateLessorForAddToWishlistCommandHandlerTest(accountId)));
        // Act & Assert
        await Assert.ThrowsAsync<WishlistException.ProductBelongsToUserException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ProductNotInWishlist_AddsToWishlist()
    {
        // Arrange
        var command = new Command.AddToWishlistCommand
        (
            AccountId: Guid.NewGuid(),
            ProductId: Guid.NewGuid()
        );

        // Mock the AccountRepository to simulate account is existed.
        _mockEFUnitOfWork.Setup(x => x.AccountRepository.FindByIdAsync(
            command.AccountId,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Account, object>>[]>()
        )).ReturnsAsync(new Account());
        // Mock the ProductRepository to simulate product is existed.
        _mockEFUnitOfWork.Setup(x => x.ProductRepository.FindByIdAsync(
            command.ProductId,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Product, object>>[]>()
        )).ReturnsAsync(Product.CreateProductForAddToWishlistCommandHandlerTest(Lessor.CreateLessorForAddToWishlistCommandHandlerTest(Guid.NewGuid())));
        // Mock the WishlistRepository to simulate product is not added to wishlist.
        _mockEFUnitOfWork.Setup(x => x.WishlistRepository.FindSingleAsync(
            It.IsAny<Expression<Func<Wishlist, bool>>>(),            
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Wishlist, object>>[]>()
        )).ReturnsAsync((Wishlist?) null);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mockEFUnitOfWork.Verify(x => x.WishlistRepository.Add(It.IsAny<Wishlist>()), Times.Once);
        _mockEFUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ProductInWishlist_RemovesFromWishlist()
    {
        // Arrange
        var command = new Command.AddToWishlistCommand
        (
            AccountId: Guid.NewGuid(),
            ProductId: Guid.NewGuid()
        );

        // Mock the AccountRepository to simulate account is existed.
        _mockEFUnitOfWork.Setup(x => x.AccountRepository.FindByIdAsync(
            command.AccountId,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Account, object>>[]>()
        )).ReturnsAsync(new Account());
        // Mock the ProductRepository to simulate product is existed.
        _mockEFUnitOfWork.Setup(x => x.ProductRepository.FindByIdAsync(
            command.ProductId,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Product, object>>[]>()
        )).ReturnsAsync(Product.CreateProductForAddToWishlistCommandHandlerTest(Lessor.CreateLessorForAddToWishlistCommandHandlerTest(Guid.NewGuid())));
        // Mock the WishlistRepository to simulate product is added to wishlist.
        _mockEFUnitOfWork.Setup(x => x.WishlistRepository.FindSingleAsync(
            It.IsAny<Expression<Func<Wishlist, bool>>>(),        
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Wishlist, object>>[]>()
        )).ReturnsAsync(new Wishlist());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mockEFUnitOfWork.Verify(x => x.WishlistRepository.Remove(It.IsAny<Wishlist>()), Times.Once);
        _mockEFUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
