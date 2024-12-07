using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Contract.Enumarations.Order;
using Neighbor.Contract.Services.Orders;
using Neighbor.Domain.Abstraction.EntitiyFramework;
using Neighbor.Domain.Entities;
using Neighbor.Domain.Exceptions;
using Neighbor.Application.UseCases.V2.Commands.Orders;
using MediatR;
using System.Linq.Expressions;
using Neighbor.Contract.DTOs.PaymentDTOs;
using Microsoft.Identity.Client;

namespace Neighbor.Application.UnitTests.UseCases.V2.Commands.Orders
{
    public class UserConfirmOrderCommandHandlerTests
    {
        private readonly Mock<IEFUnitOfWork> _mockEFUnitOfWork;
        private readonly Mock<IPublisher> _mockPublisher;
        private readonly UserConfirmOrderCommandHandler _handler;

        public UserConfirmOrderCommandHandlerTests()
        {
            _mockEFUnitOfWork = new Mock<IEFUnitOfWork>();
            _mockPublisher = new Mock<IPublisher>();
            _handler = new UserConfirmOrderCommandHandler(_mockEFUnitOfWork.Object, _mockPublisher.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowOrderNotFoundException_WhenOrderNotFound()
        {
            // Arrange
            var request = new Command.UserConfirmOrderCommand
            (
                AccountId: Guid.NewGuid(),
                OrderId: Guid.NewGuid(),
                IsApproved: true,
                RejectReason: null
            );

            // Mocking OrderRepository to return null (simulating order not found)
            _mockEFUnitOfWork.Setup(x => x.OrderRepository.FindByIdAsync(
            request.OrderId,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Order, object>>[]>()
        )).ReturnsAsync((Order?)null);

            // Act & Assert
            await Assert.ThrowsAsync<OrderException.OrderNotFoundException>(() =>
                _handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowOrderNotBelongToUserException_WhenOrderDoesNotBelongToUser()
        {
            // Arrange
            var request = new Command.UserConfirmOrderCommand
            (
                OrderId: Guid.NewGuid(),
                AccountId: Guid.NewGuid(),
                IsApproved: true,
                RejectReason: null
            );

            var order = Order.CreateOrderWithIdAndAccountAndOrderStatus(Guid.NewGuid(), Account.CreateAccountWithAccountId(Guid.NewGuid()), OrderStatusType.Pending);
            // Mocking OrderRepository to return Order Not Belong To User
            _mockEFUnitOfWork.Setup(x => x.OrderRepository.FindByIdAsync(
            request.OrderId,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Order, object>>[]>()
        )).ReturnsAsync(order);

            // Act & Assert
            await Assert.ThrowsAsync<OrderException.OrderNotBelongToUserException>(() =>
                _handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldUpdateOrderStatusAndPublishEvent_WhenOrderIsApproved()
        {
            var accountId = Guid.NewGuid();
            // Arrange
            var request = new Command.UserConfirmOrderCommand
            (
                AccountId: accountId,
                OrderId: Guid.NewGuid(),
                IsApproved: true,
                RejectReason: null
            );

            var order = Order.CreateOrderWithIdAndAccountAndOrderStatusAndProduct
            (Guid.NewGuid(), Account.CreateAccountWithAccountId(accountId), OrderStatusType.Pending, Product.CreateProductWithNameAndLessors("Test Product", Lessor.CreateLessorWithAccount(Account.CreateAccountWithEmail("test@domain.com"))));

            // Mocking OrderRepository to return Valid Order.
            _mockEFUnitOfWork.Setup(x => x.OrderRepository.FindByIdAsync(
            request.OrderId,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Order, object>>[]>()
        )).ReturnsAsync(order);

            // Mocking save changes
            _mockEFUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            var successResponse = result as Result<Success>;
            Assert.Equal(MessagesList.OrderConfirmSuccessfully.GetMessage().Message, successResponse?.Value.Message);

            _mockEFUnitOfWork.Verify(x => x.OrderRepository.Update(order), Times.Once);
            _mockEFUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            _mockPublisher.Verify(x => x.Publish(
                It.IsAny<DomainEvent.NotiLessorAboutUserApprovedOrderSuccess>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldUpdateOrderStatusAndPublishEvent_WhenOrderIsRejected()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var request = new Command.UserConfirmOrderCommand
            (
                AccountId: accountId,
                OrderId: Guid.NewGuid(),
                IsApproved: false,
                RejectReason: "Test Reject Reason"
            );

            var order = Order.CreateOrderWithIdAndAccountAndOrderStatusAndProduct
            (Guid.NewGuid(), Account.CreateAccountWithAccountId(accountId), OrderStatusType.Pending, Product.CreateProductWithNameAndLessors("Test Product", Lessor.CreateLessorWithAccount(Account.CreateAccountWithEmail("test@domain.com"))));

            // Mocking OrderRepository to return Valid Order.
            _mockEFUnitOfWork.Setup(x => x.OrderRepository.FindByIdAsync(
            request.OrderId,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Order, object>>[]>()
        )).ReturnsAsync(order);

            // Mocking save changes
            _mockEFUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            var successResponse = result as Result<Success>;
            Assert.Equal(MessagesList.OrderConfirmSuccessfully.GetMessage().Message, successResponse?.Value.Message);

            _mockEFUnitOfWork.Verify(x => x.OrderRepository.Update(order), Times.Once);
            _mockEFUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            _mockPublisher.Verify(x => x.Publish(
                It.IsAny<DomainEvent.NotiLessorAboutUserRejectedOrderSuccess>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowOrderRejectWithoutReasonException_WhenRejectReasonIsNull()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var request = new Command.UserConfirmOrderCommand
            (
                AccountId: accountId,
                OrderId: Guid.NewGuid(),
                IsApproved: false,
                RejectReason: null
            );

            var order = Order.CreateOrderWithIdAndAccountAndOrderStatus
            (Guid.NewGuid(), Account.CreateAccountWithAccountId(accountId), OrderStatusType.Pending);

            // Mocking OrderRepository to return Valid Order.
            _mockEFUnitOfWork.Setup(x => x.OrderRepository.FindByIdAsync(
            request.OrderId,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Order, object>>[]>()
        )).ReturnsAsync(order);

            // Act & Assert
            await Assert.ThrowsAsync<OrderException.OrderRejectWithoutReasonException>(() =>
                _handler.Handle(request, CancellationToken.None));
        }
    }
}
