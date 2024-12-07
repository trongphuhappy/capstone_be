using Microsoft.Extensions.Options;
using Moq;
using Neighbor.Application.UseCases.V2.Commands.Orders;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Settings;
using Neighbor.Contract.DTOs;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Services.Orders;

namespace Neighbor.Application.UnitTests.UseCases.V2.Commands.Orders
{
    public class OrderFailCommandHandlerTests
    {
        private readonly Mock<IResponseCacheService> _mockResponseCacheService;
        private readonly Mock<IOptions<ClientSetting>> _mockClientSettings;
        private readonly OrderFailCommandHandler _handler;

        public OrderFailCommandHandlerTests()
        {
            _mockResponseCacheService = new Mock<IResponseCacheService>();
            _mockClientSettings = new Mock<IOptions<ClientSetting>>();

            // Mock client settings
            _mockClientSettings.Setup(x => x.Value).Returns(new ClientSetting
            {
                Url = "http://localhost/",
                OrderFail = "order-fail"
            });

            // Initialize the handler with mocked dependencies
            _handler = new OrderFailCommandHandler(
                _mockResponseCacheService.Object,
                _mockClientSettings.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnOrderFail_WhenOrderIsFailedSuccessfully()
        {
            // Arrange
            var orderId = 123;
            var request = new Command.OrderFailCommand(OrderId: orderId);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);

            // Assert result type
            var successResponse = result as Result<Success<Response.OrderFail>>;
            Assert.IsType<Result<Success<Response.OrderFail>>>(successResponse);

            // Assert the URL in the response
            Assert.Equal("http://localhost/order-fail", successResponse?.Value?.Data?.Url);

            // Verify the cache deletion
            _mockResponseCacheService.Verify(
                x => x.DeleteCacheResponseAsync($"order_{orderId}"),
                Times.Once
            );
        }
    }
}
