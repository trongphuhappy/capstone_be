using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using Neighbor.Application.UseCases.V2.Commands.Orders;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Contract.Enumarations.Order;
using Neighbor.Contract.Services.Orders;
using Neighbor.Domain.Abstraction.EntitiyFramework;
using Neighbor.Domain.Entities;
using Neighbor.Domain.Exceptions;
using Xunit;

namespace Neighbor.Application.UnitTests.UseCases.V2.Commands.Orders
{
    public class UserReportOrderCommandHandlerTests
    {
        private readonly Mock<IEFUnitOfWork> _mockEFUnitOfWork;
        private readonly Mock<IPublisher> _mockPublisher;
        private readonly UserReportOrderCommandHandler _handler;

        public UserReportOrderCommandHandlerTests()
        {
            _mockEFUnitOfWork = new Mock<IEFUnitOfWork>();
            _mockPublisher = new Mock<IPublisher>();
            _handler = new UserReportOrderCommandHandler(_mockEFUnitOfWork.Object, _mockPublisher.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowOrderNotFoundException_WhenOrderNotFound()
        {
            // Arrange
            var request = new Command.UserReportOrderCommand(
                OrderId: Guid.NewGuid(),
                AccountId: Guid.NewGuid(),
                UserReport: "Test Report"
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
            var request = new Command.UserReportOrderCommand(
                OrderId: Guid.NewGuid(),
                AccountId: Guid.NewGuid(),
                UserReport: "Test Report"
            );

            var order = Order.CreateOrderWithIdAndAccountAndOrderStatus(
                Guid.NewGuid(),
                Account.CreateAccountWithAccountId(Guid.NewGuid()), // Different account ID
                OrderStatusType.CompletedRented
            );

            // Mocking OrderRepository to return order that does not belong to user
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
        public async Task Handle_ShouldThrowOrderLessorHasNotConfirmException_WhenOrderStatusIsInvalid()
        {
            // Arrange
            var request = new Command.UserReportOrderCommand(
                OrderId: Guid.NewGuid(),
                AccountId: Guid.NewGuid(),
                UserReport: "Test Report"
            );

            var order = Order.CreateOrderWithIdAndAccountAndOrderStatus(
                Guid.NewGuid(),
                Account.CreateAccountWithAccountId(request.AccountId),
                OrderStatusType.Pending // Invalid status
            );

            // Mocking OrderRepository to return order with status is invalid
            _mockEFUnitOfWork.Setup(x => x.OrderRepository.FindByIdAsync(
            request.OrderId,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Order, object>>[]>()
            )).ReturnsAsync(order);

            // Act & Assert
            await Assert.ThrowsAsync<OrderException.OrderLessorHasNotConfirmException>(() =>
                _handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldUpdateOrderAndPublishEvent_WhenOrderIsValid()
        {
            // Arrange
            var request = new Command.UserReportOrderCommand(
                OrderId: Guid.NewGuid(),
                AccountId: Guid.NewGuid(),
                UserReport: "Valid User Report"
            );

            var account = Account.CreateAccountWithAccountId(request.AccountId);
            var product = Product.CreateProductWithNameAndLessors("Test Product", Lessor.CreateLessorWithAccount(Account.CreateAccountWithEmail("lessor@test.com")));
            var order = Order.CreateOrderWithIdAndAccountAndOrderStatusAndProduct(
                Guid.NewGuid(),
                account,
                OrderStatusType.CompletedRented,
                product
            );

            // Mocking OrderRepository to return valid order
            _mockEFUnitOfWork.Setup(x => x.OrderRepository.FindByIdAsync(
            request.OrderId,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Order, object>>[]>()
            )).ReturnsAsync(order);

            _mockEFUnitOfWork
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            var successResponse = result as Result<Success>;
            Assert.Equal(MessagesList.OrderReportSuccessfully.GetMessage().Message, successResponse?.Value.Message);

            _mockEFUnitOfWork.Verify(x => x.OrderRepository.Update(order), Times.Once);
            _mockEFUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            _mockPublisher.Verify(x => x.Publish(
                It.IsAny<DomainEvent.NotiLessorAboutUserReportedOrderSuccess>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
