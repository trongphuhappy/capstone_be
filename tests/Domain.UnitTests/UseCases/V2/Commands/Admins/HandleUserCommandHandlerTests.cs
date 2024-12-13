using Moq;
using Xunit;
using System.Threading;
using System.Threading.Tasks;
using Neighbor.Application.UseCases.V2.Commands.Admins;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Domain.Abstraction.EntitiyFramework;
using Neighbor.Domain.Entities;
using Neighbor.Domain.Exceptions;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Services.Admins;
using MediatR;
using System.Linq.Expressions;

namespace Neighbor.Application.Tests.UseCases.V2.Commands.Admins
{
    public class HandleUserCommandHandlerTests
    {
        private readonly Mock<IEFUnitOfWork> _mockEFUnitOfWork;
        private readonly Mock<IPublisher> _mockPublisher;
        private readonly Mock<IMediaService> _mockMediaService;
        private readonly HandleUserCommandHandler _handler;

        public HandleUserCommandHandlerTests()
        {
            _mockEFUnitOfWork = new Mock<IEFUnitOfWork>();
            _mockPublisher = new Mock<IPublisher>();
            _mockMediaService = new Mock<IMediaService>();

            _handler = new HandleUserCommandHandler(
                _mockEFUnitOfWork.Object,
                _mockPublisher.Object,
                _mockMediaService.Object
            );
        }

        [Fact]
        public async Task Handle_WhenAccountNotFound_ShouldThrowAccountNotFoundException()
        {
            // Arrange
            var command = new Command.HandleUserCommand(Guid.NewGuid(), true, "Inappropriate");

            // Mock the AccountRepository to simulate account not found
            _mockEFUnitOfWork.Setup(x => x.AccountRepository.FindByIdAsync(
                command.AccountId,
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Account, object>>[]>()
            )).ReturnsAsync((Account?)null);

            // Act & Assert
            await Assert.ThrowsAsync<AccountException.AccountNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WhenAccountIsAlreadyBanned_ShouldThrowAccountHasAlreadyBannedException()
        {
            // Arrange
            var account = new Account { Id = Guid.NewGuid(), IsDeleted = true };
            var command = new Command.HandleUserCommand(Guid.NewGuid(), true, "Inappropriate");

            // Mock the AccountRepository to simulate account has already banned
            _mockEFUnitOfWork.Setup(x => x.AccountRepository.FindByIdAsync(
                command.AccountId,
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Account, object>>[]>()
            )).ReturnsAsync(account);

            // Act & Assert
            await Assert.ThrowsAsync<AccountException.AccountHasAlreadyBannedException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WhenAccountIsAlreadyUnbanned_ShouldThrowAccountHasAlreadyUnbannedException()
        {
            // Arrange
            var account = new Account { Id = Guid.NewGuid(), IsDeleted = false };
            var command = new Command.HandleUserCommand(Guid.NewGuid(), false, "No Reason");

            // Mock the AccountRepository to simulate account has already unbanned
            _mockEFUnitOfWork.Setup(x => x.AccountRepository.FindByIdAsync(
                command.AccountId,
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Account, object>>[]>()
            )).ReturnsAsync(account);

            // Act & Assert
            await Assert.ThrowsAsync<AccountException.AccountHasAlreadyUnbannedException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WhenBanIsRequestedWithoutReason_ShouldThrowBanWithNoReasonException()
        {
            // Arrange
            var account = new Account { Id = Guid.NewGuid(), IsDeleted = false };
            var command = new Command.HandleUserCommand(Guid.NewGuid(), true, null);

            // Mock the AccountRepository to simulate account is valid
            _mockEFUnitOfWork.Setup(x => x.AccountRepository.FindByIdAsync(
                command.AccountId,
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Account, object>>[]>()
            )).ReturnsAsync(account);

            // Act & Assert
            await Assert.ThrowsAsync<AdminException.BanWithNoReasonException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WhenAccountIsBanned_ShouldSendBanEmailAndReturnSuccess()
        {
            // Arrange
            var account = Account.CreateAccountWithEmailAndIsDeleted("test@gmail.com", false);
            var command = new Command.HandleUserCommand(Guid.NewGuid(), true, "Inappropriate");

            // Mock the AccountRepository to simulate account is valid
            _mockEFUnitOfWork.Setup(x => x.AccountRepository.FindByIdAsync(
                command.AccountId,
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Account, object>>[]>()
            )).ReturnsAsync(account);

            _mockEFUnitOfWork.Setup(uow => uow.AccountRepository.Update(It.IsAny<Account>()));
            _mockEFUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _mockPublisher.Verify(p => p.Publish(It.IsAny<DomainEvent.AccountHasBeenBanned>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenAccountIsUnbanned_ShouldSendUnbanEmailAndReturnSuccess()
        {
            // Arrange
            var account = Account.CreateAccountWithEmailAndIsDeleted("test@gmail.com", true);
            var command = new Command.HandleUserCommand(Guid.NewGuid(), false, null);

            // Mock the AccountRepository to simulate account is valid
            _mockEFUnitOfWork.Setup(x => x.AccountRepository.FindByIdAsync(
                command.AccountId,
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Account, object>>[]>()
            )).ReturnsAsync(account);

            _mockEFUnitOfWork.Setup(uow => uow.AccountRepository.Update(It.IsAny<Account>()));
            _mockEFUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _mockPublisher.Verify(p => p.Publish(It.IsAny<DomainEvent.AccountHasBeenUnbanned>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
