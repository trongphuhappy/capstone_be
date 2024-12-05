using Moq;
using Neighbor.Application.UseCases.V2.Commands.Products;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Services.Products;
using Neighbor.Domain.Abstraction.EntitiyFramework;
using Neighbor.Domain.Entities;
using Neighbor.Domain.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Neighbor.Contract.Enumarations.Product;
using Neighbor.Contract.Enumarations.MessagesList;
using MediatR;
using static Neighbor.Contract.Services.Products.DomainEvent;
using System.Linq.Expressions;
namespace Neighbor.Application.UnitTests.UseCases.V2.Commands.Products;

public class ConfirmProductCommandHandlerTests
{
    private readonly Mock<IEFUnitOfWork> _mockEFUnitOfWork;
    private readonly Mock<IPublisher> _mockPublisher;
    private readonly ConfirmProductCommandHandler _handler;

    public ConfirmProductCommandHandlerTests()
    {
        _mockEFUnitOfWork = new Mock<IEFUnitOfWork>();
        _mockPublisher = new Mock<IPublisher>();
        _handler = new ConfirmProductCommandHandler(_mockEFUnitOfWork.Object, _mockPublisher.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowProductNotFoundException_WhenProductDoesNotExist()
    {
        // Arrange
        var command = new Command.ConfirmProductCommand
        (
            ProductId: Guid.NewGuid(),
            ConfirmStatus: ConfirmStatus.Approved,
            RejectReason: ""
        );

        // Mock the ProductRepository to simulate product not found
        _mockEFUnitOfWork.Setup(x => x.ProductRepository.FindByIdAsync(
            command.ProductId,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Product, object>>[]>()
        )).ReturnsAsync((Product?)null);
        // Act & Assert
        await Assert.ThrowsAsync<ProductException.ProductNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );
    }

    [Fact]
    public async Task Handle_ShouldThrowProductHasAlreadyApprovedException_WhenProductIsAlreadyApproved()
    {
        // Arrange
        var product = new Product("", StatusType.Available, "", "", 0, 0, 0, 0, 0, ConfirmStatus.Approved, Guid.NewGuid());
        
        var command = new Command.ConfirmProductCommand
        (
            ProductId: Guid.NewGuid(),
            ConfirmStatus: ConfirmStatus.Approved,
            RejectReason: ""
        );

        // Mock the ProductRepository to simulate product is already approved
        _mockEFUnitOfWork.Setup(x => x.ProductRepository.FindByIdAsync(
            command.ProductId,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Product, object>>[]>()
        )).ReturnsAsync(product);

        // Act & Assert
        await Assert.ThrowsAsync<ProductException.ProductHasAlreadyApprovedException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );
    }

    [Fact]
    public async Task Handle_ShouldThrowProductHasAlreadyRejectedException_WhenProductIsAlreadyRejected()
    {
        // Arrange
        var product = new Product("", StatusType.Available, "", "", 0, 0, 0, 0, 0, ConfirmStatus.Rejected, Guid.NewGuid());
        
        var command = new Command.ConfirmProductCommand
        (
            ProductId: Guid.NewGuid(),
            ConfirmStatus: ConfirmStatus.Rejected,
            RejectReason: ""
        );

        // Mock the ProductRepository to simulate product is already rejected
        _mockEFUnitOfWork.Setup(x => x.ProductRepository.FindByIdAsync(
            command.ProductId,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Product, object>>[]>()
        )).ReturnsAsync(product);

        // Act & Assert
        await Assert.ThrowsAsync<ProductException.ProductHasAlreadyRejectedException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowProductRejectNoReasonException_WhenProductIsRejectedWithoutReason()
    {
        // Arrange
        var product = new Product("", StatusType.Available, "", "", 0, 0, 0, 0, 0, ConfirmStatus.Pending, Guid.NewGuid());
        var command = new Command.ConfirmProductCommand
        (
            ProductId: Guid.NewGuid(),
            ConfirmStatus: ConfirmStatus.Rejected,
            RejectReason: null
        );

        // Mock the ProductRepository to simulate product is existed.
        _mockEFUnitOfWork.Setup(x => x.ProductRepository.FindByIdAsync(
            command.ProductId,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Product, object>>[]>()
        )).ReturnsAsync(product);


        // Act & Assert
        await Assert.ThrowsAsync<ProductException.ProductRejectNoReasonException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );
    }

    [Fact]
    public async Task Handle_ShouldUpdateProductAndSendApprovalEmail_WhenProductIsApproved()
    {
        // Arrange
        var product = Product.CreateProductForConfirmProductCommandHandlerTest(Lessor.CreateLessorForConfirmProductCommandHandlerTest(Account.CreateAccountForConfirmProductCommandHandlerTest("trongphuhappy@gmail.com")));

        var command = new Command.ConfirmProductCommand
        (
            ProductId: Guid.NewGuid(),
            ConfirmStatus: ConfirmStatus.Approved,
            RejectReason: null
        );

        // Mock the ProductRepository to simulate product is valid
        _mockEFUnitOfWork.Setup(x => x.ProductRepository.FindByIdAsync(
            command.ProductId,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Product, object>>[]>()
        )).ReturnsAsync(product);
        _mockEFUnitOfWork.Setup(x => x.ProductRepository.Update(It.IsAny<Product>())).Verifiable();
        _mockEFUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mockEFUnitOfWork.Verify(x => x.ProductRepository.Update(It.IsAny<Product>()), Times.Once);
        _mockPublisher.Verify(p => p.Publish(It.IsAny<ProductHasBeenApproved>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldUpdateProductAndSendRejectionEmail_WhenProductIsRejected()
    {
        // Arrange
        var product = Product.CreateProductForConfirmProductCommandHandlerTest(Lessor.CreateLessorForConfirmProductCommandHandlerTest(Account.CreateAccountForConfirmProductCommandHandlerTest("trongphuhappy@gmail.com")));

        var command = new Command.ConfirmProductCommand
        (
            ProductId: Guid.NewGuid(),
            ConfirmStatus: ConfirmStatus.Rejected,
            RejectReason: "Not Valid Product"
        );

        // Mock the ProductRepository to simulate product is valid
        _mockEFUnitOfWork.Setup(x => x.ProductRepository.FindByIdAsync(
            command.ProductId,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Product, object>>[]>()
        )).ReturnsAsync(product);
        _mockEFUnitOfWork.Setup(x => x.ProductRepository.Update(It.IsAny<Product>())).Verifiable();
        _mockEFUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mockEFUnitOfWork.Verify(x => x.ProductRepository.Update(It.IsAny<Product>()), Times.Once);
        _mockPublisher.Verify(p => p.Publish(It.IsAny<ProductHasBeenRejected>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
