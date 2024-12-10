using Moq;
using Neighbor.Application.UseCases.V2.Commands.Feedbacks;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Contract.Services.Feedbacks;
using Neighbor.Domain.Abstraction.EntitiyFramework;
using Neighbor.Domain.Entities;
using Neighbor.Domain.Exceptions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Neighbor.Application.UnitTests.UseCases.V2.Commands.Feedbacks;


public class CreateFeedbackCommandHandlerTests
{
    private readonly Mock<IEFUnitOfWork> _mockEFUnitOfWork;
    private readonly CreateFeedbackCommandHandler _handler;

    public CreateFeedbackCommandHandlerTests()
    {
        _mockEFUnitOfWork = new Mock<IEFUnitOfWork>();
        _handler = new CreateFeedbackCommandHandler(_mockEFUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_CreatesFeedbackAndReturnsSuccess()
    {
        // Arrange
        var command = new Command.CreateFeedbackCommand
        (
            AccountId: Guid.NewGuid(),
            OrderId: Guid.NewGuid(),
            ProductId: Guid.NewGuid(),
            Content: "Great product!"
        );

        _mockEFUnitOfWork.Setup(u => u.FeedbackRepository.GetFeedbackByAccountId(It.IsAny<Guid>()))
            .ReturnsAsync((Feedback)null);

        _mockEFUnitOfWork.Setup(u => u.FeedbackRepository.Add(It.IsAny<Feedback>()));
        _mockEFUnitOfWork
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        var successResponse = result as Result<Success>;
        Assert.Equal(MessagesList.CreateFeedbackSuccessfully.GetMessage().Code, successResponse.Value?.Code);
        Assert.Equal(MessagesList.CreateFeedbackSuccessfully.GetMessage().Message, successResponse.Value?.Message);

        _mockEFUnitOfWork.Verify(u => u.FeedbackRepository.Add(It.IsAny<Feedback>()), Times.Once);
        _mockEFUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ExistingFeedback_ThrowsAccountHasFeedbackException()
    {
        // Arrange
        var command = new Command.CreateFeedbackCommand
        (
            AccountId: Guid.NewGuid(),
            OrderId: Guid.NewGuid(),
            ProductId: Guid.NewGuid(),
            Content: "Great product!"
        );

        var existingFeedback = Feedback.CreateFeedback(command.OrderId, command.AccountId, command.ProductId, "Existing feedback");

        _mockEFUnitOfWork.Setup(u => u.FeedbackRepository.GetFeedbackByAccountId(It.IsAny<Guid>()))
            .ReturnsAsync(existingFeedback);

        // Act & Assert
        await Assert.ThrowsAsync<FeedbackException.AccountHasFeedbackException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _mockEFUnitOfWork.Verify(u => u.FeedbackRepository.Add(It.IsAny<Feedback>()), Times.Never);
        _mockEFUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
