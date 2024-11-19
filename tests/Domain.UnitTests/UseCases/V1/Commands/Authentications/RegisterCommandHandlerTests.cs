using MediatR;
using Moq;
using Neighbor.Application.UseCases.V1.Commands.Authentications;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Enumarations.Authentication;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Contract.Services.Authentications;
using Neighbor.Domain.Abstraction.EntitiyFramework;
using Neighbor.Domain.Entities;
using Neighbor.Domain.Exceptions;
using System.Linq.Expressions;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IResponseCacheService> _mockResponseCacheService;
    private readonly Mock<IEFUnitOfWork> _mockEFUnitOfWork;
    private readonly Mock<IPublisher> _mockPublisher;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _mockResponseCacheService = new Mock<IResponseCacheService>();
        _mockEFUnitOfWork = new Mock<IEFUnitOfWork>();
        _mockPublisher = new Mock<IPublisher>();
        _handler = new RegisterCommandHandler(_mockResponseCacheService.Object, _mockEFUnitOfWork.Object, _mockPublisher.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowEmailExistException_WhenEmailAlreadyExists()
    {
        // Arrange
        var command = new Neighbor.Contract.Services.Authentications.Command.RegisterCommand("firstname", "lastname", "vietvyqw@gmail.com", "123123123", "123123123", GenderType.Male);

        // Mock user exist
        _mockEFUnitOfWork.Setup(x => x.AccountRepository.AnyAsync(
            It.IsAny<Expression<Func<Account, bool>>?>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Account, object>>[]>()
        )).ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AuthenticationException.EmailExistException>(() =>
             _handler.Handle(command, CancellationToken.None)
         );

        // Assert: Check if exception message and code are as expected
        var expectedMessage = MessagesList.AuthEmailExistException.GetMessage().Message;
        var expectedCode = MessagesList.AuthEmailExistException.GetMessage().Code;

        Assert.Equal(expectedMessage, exception.Message);
        Assert.Equal(expectedCode, exception.ErrorCode);
    }

    [Fact]
    public async Task Handle_ShouldCacheUserData_WhenEmailDoesNotExist()
    {
        // Arrange
        var command = new Neighbor.Contract.Services.Authentications.Command.RegisterCommand("firstname", "lastname", "phus@gmail.com", "123123123", "123123123", GenderType.Male);

        // Mock user not exist
        _mockEFUnitOfWork.Setup(x => x.AccountRepository.AnyAsync(
            It.IsAny<Expression<Func<Account, bool>>?>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Account, object>>[]>()
        )).ReturnsAsync(false);

        // Mock: Response Cache Service
        _mockResponseCacheService.Setup(x => x.SetCacheResponseAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<TimeSpan>()
        )).Returns(Task.CompletedTask);

        // Mock: Publisher
        _mockPublisher.Setup(x => x.Publish(It.IsAny<DomainEvent.UserRegistedWithLocal>(), It.IsAny<CancellationToken>()))
                      .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert: Check result
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);

        var successMessage = ((Result<Success>)result).Value;
        Assert.Equal(MessagesList.AuthRegisterSuccess.GetMessage().Message, successMessage.Message);
        Assert.Equal(MessagesList.AuthRegisterSuccess.GetMessage().Code, successMessage.Code);
    }
}
