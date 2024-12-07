using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Neighbor.Application.UseCases.V2.Commands.Orders;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Contract.Enumarations.Order;
using Neighbor.Contract.Enumarations.Product;
using Neighbor.Domain.Abstraction.EntitiyFramework;
using Neighbor.Domain.Entities;
using Neighbor.Domain.Exceptions;
using MediatR;
using Neighbor.Contract.Services.Orders;
using System.Linq.Expressions;
using Azure.Core;
using Microsoft.Identity.Client;

public class LessorConfirmOrderCommandHandlerTests
{
    private readonly Mock<IEFUnitOfWork> _mockEFUnitOfWork;
    private readonly Mock<IPublisher> _mockPublisher;
    private readonly LessorConfirmOrderCommandHandler _handler;

    public LessorConfirmOrderCommandHandlerTests()
    {
        _mockEFUnitOfWork = new Mock<IEFUnitOfWork>();
        _mockPublisher = new Mock<IPublisher>();
        _handler = new LessorConfirmOrderCommandHandler(_mockEFUnitOfWork.Object, _mockPublisher.Object);
    }

    [Fact]
    public async Task Handle_OrderNotFound_ShouldThrowOrderNotFoundException()
    {
        // Arrange
        var command = new Command.LessorConfirmOrderCommand
        (
            AccountId: Guid.NewGuid(),
            OrderId: Guid.NewGuid(),
            IsApproved: true,
            RejectReason: null
        );

        // Mocking OrderRepository to return null (simulating order not found)
        _mockEFUnitOfWork.Setup(x => x.OrderRepository.FindByIdAsync(
        command.OrderId,
        It.IsAny<CancellationToken>(),
        It.IsAny<Expression<Func<Order, object>>[]>()
    )).ReturnsAsync((Order?)null);

        // Act & Assert
        await Assert.ThrowsAsync<OrderException.OrderNotFoundException>(
            () => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_OrderNotBelongToUser_ShouldThrowOrderNotBelongToUserException()
    {
        // Arrange
        var command = new Command.LessorConfirmOrderCommand
        (
            AccountId: Guid.NewGuid(),
            OrderId: Guid.NewGuid(),
            IsApproved: true,
            RejectReason: null
        );

        var order = Order.CreateOrderWithIdAndAccountAndOrderStatusAndProduct
            (Guid.NewGuid(), Account.CreateAccountWithAccountId(Guid.NewGuid()), OrderStatusType.Pending, Product.CreateProductWithNameAndLessors("Test Product", Lessor.CreateLessorWithAccount(Account.CreateAccountWithEmail("test@domain.com"))));
        // Mocking OrderRepository to return Order Not Belong To User
        _mockEFUnitOfWork.Setup(x => x.OrderRepository.FindByIdAsync(
        command.OrderId,
        It.IsAny<CancellationToken>(),
        It.IsAny<Expression<Func<Order, object>>[]>()
    )).ReturnsAsync(order);

        // Act & Assert
        await Assert.ThrowsAsync<OrderException.OrderNotBelongToUserException>(
            () => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ValidCommand_OrderStatusShouldBeUpdatedAndEmailSent()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var command = new Command.LessorConfirmOrderCommand
        (
            AccountId: accountId,
            OrderId: orderId,
            IsApproved: true,
            RejectReason: null
        );

        var order = Order.CreateOrderWithIdAndAccountAndOrderStatusAndProduct
            (Guid.NewGuid(), Account.CreateAccountWithEmail("test@domain.example"), OrderStatusType.UserApproved, Product.CreateProductWithIdAndLessors(accountId, Lessor.CreateLessorWithAccount(Account.CreateAccountWithAccountId(accountId))));

        // Mocking OrderRepository to return Valid Order
        _mockEFUnitOfWork.Setup(x => x.OrderRepository.FindByIdAsync(
        command.OrderId,
        It.IsAny<CancellationToken>(),
        It.IsAny<Expression<Func<Order, object>>[]>()
    )).ReturnsAsync(order);
        // Mocking ProductRepository to return Valid Product
        _mockEFUnitOfWork.Setup(x => x.ProductRepository.FindByIdAsync(
        command.OrderId,
        It.IsAny<CancellationToken>(),
        It.IsAny<Expression<Func<Product, object>>[]>()
    )).ReturnsAsync(new Product());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockEFUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        _mockPublisher.Verify(x => x.Publish(
                It.IsAny<DomainEvent.NotiUserAboutLessorApprovedOrderSuccess>(),
                It.IsAny<CancellationToken>()), Times.Once);
        var successResponse = result as Result<Success>;

        Assert.Equal(MessagesList.OrderConfirmSuccessfully.GetMessage().Code, successResponse?.Value.Code);
        Assert.Equal(MessagesList.OrderConfirmSuccessfully.GetMessage().Message, successResponse?.Value.Message);
    }

    [Fact]
    public async Task Handle_RejectWithoutReason_ShouldThrowOrderRejectWithoutReasonException()
    {
        // Arrange
        var command = new Command.LessorConfirmOrderCommand
        (
            AccountId: Guid.NewGuid(),
            OrderId: Guid.NewGuid(),
            IsApproved: false,
            RejectReason: null
        );

        var order = Order.CreateOrderWithIdAndProductAndOrderStatus
            (Guid.NewGuid(), Product.CreateProductWithIdAndLessors(Guid.NewGuid(), Lessor.CreateLessorWithAccount(Account.CreateAccountWithAccountId(command.AccountId))), OrderStatusType.UserApproved);

        // Mocking OrderRepository to return Valid Order
        _mockEFUnitOfWork.Setup(x => x.OrderRepository.FindByIdAsync(
        command.OrderId,
        It.IsAny<CancellationToken>(),
        It.IsAny<Expression<Func<Order, object>>[]>()
    )).ReturnsAsync(order);

        // Act & Assert
        await Assert.ThrowsAsync<OrderException.OrderRejectWithoutReasonException>(
            () => _handler.Handle(command, CancellationToken.None));
    }
}
