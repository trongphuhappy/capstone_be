using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.Extensions.Options;
using Moq;
using Neighbor.Application.UseCases.V2.Commands.Orders;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.DTOs.PaymentDTOs;
using Neighbor.Contract.Enumarations.Product;
using Neighbor.Contract.Services.Orders;
using Neighbor.Domain.Abstraction.EntitiyFramework;
using Neighbor.Domain.Entities;
using Neighbor.Domain.Exceptions;
using System.Linq.Expressions;
namespace Neighbor.Application.UnitTests.UseCases.V2.Commands.Orders;
public class CreateOrderBankingCommandHandlerTests
{
    private readonly Mock<IEFUnitOfWork> _mockEFUnitOfWork;
    private readonly Mock<IPaymentService> _mockPaymentService;
    private readonly Mock<IResponseCacheService> _mockResponseCacheService;
    private readonly CreateOrderBankingCommandHandler _handler;
    private readonly PayOSSetting _payOSSetting;

    public CreateOrderBankingCommandHandlerTests()
    {
        _mockEFUnitOfWork = new Mock<IEFUnitOfWork>();
        _mockPaymentService = new Mock<IPaymentService>();
        _mockResponseCacheService = new Mock<IResponseCacheService>();
        _payOSSetting = new PayOSSetting
        {
            SuccessUrl = "https://example.com/success",
            ErrorUrl = "https://example.com/error"
        };

        _handler = new CreateOrderBankingCommandHandler(
            _mockPaymentService.Object,
            Options.Create(_payOSSetting),
            _mockResponseCacheService.Object,
            _mockEFUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccessResult()
    {
        // Arrange
        var command = new Command.CreateOrderBankingCommand
        (
            AccountId: Guid.NewGuid(),
            ProductId: Guid.NewGuid(),
            RentTime: DateTime.UtcNow,
            ReturnTime: DateTime.UtcNow
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
        )).ReturnsAsync(Product.CreateProductForCreateOrderBankingCommandHandlerTest(Lessor.CreateLessorForAddToWishlistCommandHandlerTest(Guid.NewGuid())));
        // Mock the OrderRepository to simulate product is not ordered by user.
        _mockEFUnitOfWork.Setup(x => x.OrderRepository.AnyAsync(
            It.IsAny<Expression<Func<Order, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Order, object>>[]>()
        )).ReturnsAsync(false);
    
        var paymentResponse = new CreatePaymentResponseDTO
        (
            success: true,
            paymentUrl: "https://payment.com/12345",
            message: ""
        );
        _mockPaymentService.Setup(x => x.CreatePaymentLink(It.IsAny<CreatePaymentDTO>()))
            .ReturnsAsync(paymentResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result);
        var successResponse = result as Result<Success<CreatePaymentResponseDTO>>;
                  Assert.IsType<Result<Success<CreatePaymentResponseDTO>>>(successResponse);
        Assert.Equal(paymentResponse.PaymentUrl, successResponse?.Value?.Data?.PaymentUrl);

        _mockResponseCacheService.Verify(x => x.SetCacheResponseAsync(It.IsAny<string>(), command, TimeSpan.FromMinutes(60)), Times.Once);
    }

    [Fact]
    public async Task Handle_ProductNotFound_ThrowsProductNotFoundException()
    {
        // Arrange
        var command = new Command.CreateOrderBankingCommand
                (
                    AccountId: Guid.NewGuid(),
                    ProductId: Guid.NewGuid(),
                    RentTime: DateTime.UtcNow,
                    ReturnTime: DateTime.UtcNow
                );

        // Mock the AccountRepository to simulate account is existed.
        _mockEFUnitOfWork.Setup(x => x.AccountRepository.FindByIdAsync(
            command.AccountId,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Account, object>>[]>()
        )).ReturnsAsync(new Account());

        // Mock the ProductRepository to simulate product is not found.
        _mockEFUnitOfWork.Setup(x => x.ProductRepository.FindByIdAsync(
            command.ProductId,
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

        // Arrange
        var command = new Command.CreateOrderBankingCommand
                (
                    AccountId: accountId,
                    ProductId: Guid.NewGuid(),
                    RentTime: DateTime.UtcNow,
                    ReturnTime: DateTime.UtcNow
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
        await Assert.ThrowsAsync<OrderException.ProductBelongsToUserException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

}
