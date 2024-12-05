using Moq;
using Newtonsoft.Json;
using Neighbor.Application.UseCases.V2.Commands.Orders;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Settings;
using Neighbor.Domain.Entities;
using Neighbor.Domain.Exceptions;
using Neighbor.Domain.Abstraction.EntitiyFramework;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using MediatR;
using Microsoft.Extensions.Options;
using Neighbor.Contract.Enumarations.Product;
using Neighbor.Contract.Services.Orders;
using System.Linq.Expressions;
using Neighbor.Contract.DTOs.PaymentDTOs;

namespace Neighbor.Application.UnitTests.UseCases.V2.Commands.Orders
{
    public class OrderSuccessCommandHandlerTests
    {
        private readonly Mock<IEFUnitOfWork> _mockEFUnitOfWork;
        private readonly Mock<IPublisher> _mockPublisher;
        private readonly Mock<IResponseCacheService> _mockResponseCacheService;
        private readonly Mock<IOptions<ClientSetting>> _mockClientSettings;

        private readonly OrderSuccessCommandHandler _handler;

        public OrderSuccessCommandHandlerTests()
        {
            _mockEFUnitOfWork = new Mock<IEFUnitOfWork>();
            _mockPublisher = new Mock<IPublisher>();
            _mockResponseCacheService = new Mock<IResponseCacheService>();
            _mockClientSettings = new Mock<IOptions<ClientSetting>>();

            // Setup mock client settings
            _mockClientSettings.Setup(x => x.Value).Returns(new ClientSetting
            {
                Url = "http://localhost/",
                OrderSuccess = "order-success"
            });

            _handler = new OrderSuccessCommandHandler(
                _mockEFUnitOfWork.Object,
                _mockPublisher.Object,
                _mockResponseCacheService.Object,
                _mockClientSettings.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnOrderSuccess_WhenOrderIsProcessedSuccessfully()
        {
            // Arrange
            var orderId = 123;
            var accountId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var rentTime = DateTime.UtcNow.AddDays(1);
            var returnTime = DateTime.UtcNow.AddDays(2);

            // Mocking cache retrieval
            var orderMemory = JsonConvert.SerializeObject(new Command.CreateOrderBankingCommand
            (
                ProductId: productId,
                AccountId: accountId,
                RentTime: rentTime,
                ReturnTime: returnTime
            ));
            _mockResponseCacheService.Setup(x => x.GetCacheResponseAsync(It.IsAny<string>()))
                .ReturnsAsync(orderMemory);

            // Mocking product repository
            var product = Product.CreateProductForOrderSuccessCommandHandlerTest(productId, Lessor.CreateLessorForOrderSuccessCommandHandlerTest(accountId), 100, StatusType.Available);
            _mockEFUnitOfWork.Setup(x => x.ProductRepository.FindByIdAsync(
                productId,
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Product, object>>[]>()
            )).ReturnsAsync(product);

            //// Mocking lessor repository
            //var lessor = Lessor.CreateLessorForOrderSuccessCommandHandlerTest(accountId);
            //_mockEFUnitOfWork.Setup(x => x.LessorRepository.FindByIdAsync(
            //    accountId,
            //    It.IsAny<CancellationToken>(),
            //    It.IsAny<Expression<Func<Lessor, object>>[]>()
            //)).ReturnsAsync(lessor);

            // Mocking wallet repository (passing lessor.Id instead of accountId)
            var wallet = Wallet.CreateWalletForOrderSuccessCommandHandlerTest(product.LessorId);
            _mockEFUnitOfWork.Setup(x => x.WalletRepository.GetWalletByLessorId(product.LessorId))
                .ReturnsAsync(wallet);

            // Mocking OrderRepository Add method
            _mockEFUnitOfWork.Setup(x => x.OrderRepository.Add(It.IsAny<Order>())).Verifiable();

            // Mocking save changes
            _mockEFUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(new Command.OrderSuccessCommand (OrderId: orderId), CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result);

            // Correct the expected response type
            var successResponse = result as Result<Success<Response.OrderSuccess>>;
            Assert.IsType<Result<Success<Response.OrderSuccess>>>(successResponse);

            // Validate the URL in the response
            Assert.Equal("http://localhost/order-success", successResponse?.Value?.Data?.Url);

            // Verify cache deletion
            _mockResponseCacheService.Verify(x => x.DeleteCacheResponseAsync(It.IsAny<string>()), Times.Once);

            // Verify save changes was called once
            _mockEFUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task Handle_ShouldThrowException_WhenProductNotFound()
        {
            // Arrange
            var orderId = 123;
            var accountId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            // Mocking cache retrieval
            var orderMemory = JsonConvert.SerializeObject(new Command.CreateOrderBankingCommand
            (
                ProductId: productId,
                AccountId: accountId,
                RentTime: DateTime.UtcNow.AddDays(1),
                ReturnTime: DateTime.UtcNow.AddDays(2)
            ));
            _mockResponseCacheService.Setup(x => x.GetCacheResponseAsync(It.IsAny<string>()))
                .ReturnsAsync(orderMemory);

            var orderObject = JsonConvert.DeserializeObject<Command.CreateOrderBankingCommand>(orderMemory);

            // Mocking product repository to return null (simulating product not found)
            _mockEFUnitOfWork.Setup(x => x.ProductRepository.FindByIdAsync(
            orderObject.ProductId,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Product, object>>[]>()
        )).ReturnsAsync((Product?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ProductException.ProductNotFoundException>(
                () => _handler.Handle(new Command.OrderSuccessCommand ( OrderId: orderId), CancellationToken.None)
            );
            Assert.Equal("Can not found this Product!", exception.Message);
        }
    }
}
