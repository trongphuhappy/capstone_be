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
    public class AdminConfirmOrderCommandHandlerTests
    {
        private readonly Mock<IEFUnitOfWork> _mockEFUnitOfWork;
        private readonly Mock<IPublisher> _mockPublisher;
        private readonly AdminConfirmOrderCommandHandler _handler;

        public AdminConfirmOrderCommandHandlerTests()
        {
            _mockEFUnitOfWork = new Mock<IEFUnitOfWork>();
            _mockPublisher = new Mock<IPublisher>();
            _handler = new AdminConfirmOrderCommandHandler(_mockEFUnitOfWork.Object, _mockPublisher.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowOrderNotFoundException_WhenOrderNotFound()
        {
            // Arrange
            var request = new Command.AdminConfirmOrderCommand(
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
        public async Task Handle_ShouldThrowOrderDoesNotConflictException_WhenOrderIsNotConflicted()
        {
            // Arrange
            var order = Order.CreateOrderWithOrderReportStatusType(OrderReportStatusType.NotConflict);
            var request = new Command.AdminConfirmOrderCommand(
                OrderId: Guid.NewGuid(),
                IsApproved: true,
                RejectReason: null
            );

            // Mocking OrderRepository to return order is not conflict
            _mockEFUnitOfWork.Setup(x => x.OrderRepository.FindByIdAsync(
            request.OrderId,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Order, object>>[]>()
        )).ReturnsAsync(order);

            // Act & Assert
            await Assert.ThrowsAsync<OrderException.OrderDoesNotConflictException>(() =>
                _handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowOrderHaveAlreadyConfirmedException_WhenOrderIsAlreadyConfirmed()
        {
            // Arrange
            var order = Order.CreateOrderWithOrderReportStatusType(OrderReportStatusType.ApprovedReport);
            var request = new Command.AdminConfirmOrderCommand(
                OrderId: Guid.NewGuid(),
                IsApproved: true,
                RejectReason: null
            );

            // Mocking OrderRepository to return order is already confirmed
            _mockEFUnitOfWork.Setup(x => x.OrderRepository.FindByIdAsync(
            request.OrderId,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Order, object>>[]>()
        )).ReturnsAsync(order);

            // Act & Assert
            await Assert.ThrowsAsync<OrderException.OrderHaveAlreadyConfirmedException>(() =>
                _handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowOrderRejectWithoutReasonException_WhenRejectReasonIsNull()
        {
            // Arrange
            var order = Order.CreateOrderWithOrderReportStatusType(OrderReportStatusType.PendingConflict);
            var request = new Command.AdminConfirmOrderCommand(
                OrderId: Guid.NewGuid(),
                IsApproved: false,
                RejectReason: null
            );

            // Mocking OrderRepository to return valid order
            _mockEFUnitOfWork.Setup(x => x.OrderRepository.FindByIdAsync(
            request.OrderId,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Order, object>>[]>()
        )).ReturnsAsync(order);

            // Act & Assert
            await Assert.ThrowsAsync<OrderException.OrderRejectWithoutReasonException>(() =>
                _handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldUpdateOrderAndSendEmails_WhenOrderIsApproved()
        {
            // Arrange
            var request = new Command.AdminConfirmOrderCommand(
                OrderId: Guid.NewGuid(),
                IsApproved: true,
                RejectReason: null
            );
            var order = Order.CreateOrderWithIdAndAccountAndOrderReportStatusAndProduct
            (Guid.NewGuid(), Account.CreateAccountWithAccountId(Guid.NewGuid()), OrderReportStatusType.PendingConflict, Product.CreateProductWithNameAndLessors("Test Product", Lessor.CreateLessorWithIdAndAccount(Guid.NewGuid(), Account.CreateAccountWithEmail("test@domain.com"))));

            // Mocking wallet repository (passing lessor.Id instead of accountId)
            if (order.Product != null)
            {
                var wallet = Wallet.CreateWalletForOrderSuccessCommandHandlerTest(order.Product.Lessor.Id);
                _mockEFUnitOfWork.Setup(x => x.WalletRepository.GetWalletByLessorId(order.Product.Lessor.Id))
                    .ReturnsAsync(wallet);
            }
            // Mocking OrderRepository to return valid order
            _mockEFUnitOfWork.Setup(x => x.OrderRepository.FindByIdAsync(
            request.OrderId,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Order, object>>[]>()
            )).ReturnsAsync(order);

            _mockEFUnitOfWork
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _mockEFUnitOfWork
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(MessagesList.OrderConfirmSuccessfully.GetMessage().Message, (result as Result<Success>)?.Value.Message);

            _mockEFUnitOfWork.Verify(x => x.OrderRepository.Update(order), Times.Once);
            _mockEFUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockPublisher.Verify(x => x.Publish(
                It.IsAny<DomainEvent.NotiLessorAboutAdminApprovedReportedOrderSuccess>(),
                It.IsAny<CancellationToken>()), Times.Once);
            _mockPublisher.Verify(x => x.Publish(
                It.IsAny<DomainEvent.NotiUserAboutAdminApprovedReportedOrderSuccess>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldUpdateOrderAndSendEmails_WhenOrderIsRejected()
        {
            // Arrange
            var request = new Command.AdminConfirmOrderCommand(
                OrderId: Guid.NewGuid(),
                IsApproved: false,
                RejectReason: "Test Reject Reason"
            );

            var order = Order.CreateOrderWithIdAndAccountAndOrderReportStatusAndProduct
            (Guid.NewGuid(), Account.CreateAccountWithAccountId(Guid.NewGuid()), OrderReportStatusType.PendingConflict, Product.CreateProductWithNameAndLessors("Test Product", Lessor.CreateLessorWithIdAndAccount(Guid.NewGuid(), Account.CreateAccountWithEmail("test@domain.com"))));

            // Mocking wallet repository (passing lessor.Id instead of accountId)
            if (order.Product != null)
            {
                var wallet = Wallet.CreateWalletForOrderSuccessCommandHandlerTest(order.Product.Lessor.Id);
                _mockEFUnitOfWork.Setup(x => x.WalletRepository.GetWalletByLessorId(order.Product.Lessor.Id))
                    .ReturnsAsync(wallet);
            }
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
            Assert.Equal(MessagesList.OrderConfirmSuccessfully.GetMessage().Message, (result as Result<Success>)?.Value.Message);

            _mockEFUnitOfWork.Verify(x => x.OrderRepository.Update(order), Times.Once);
            _mockEFUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockPublisher.Verify(x => x.Publish(
                It.IsAny<DomainEvent.NotiLessorAboutAdminRejectedReportedOrderSuccess>(),
                It.IsAny<CancellationToken>()), Times.Once);
            _mockPublisher.Verify(x => x.Publish(
                It.IsAny<DomainEvent.NotiUserAboutAdminRejectedReportedOrderSuccess>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
