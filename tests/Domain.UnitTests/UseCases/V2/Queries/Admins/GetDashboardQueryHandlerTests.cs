using Moq;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Neighbor.Application.UseCases.V1.Queries.Admin;
using Neighbor.Contract.Services.Admins;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Domain.Abstraction.Dappers;
using Xunit;
using Neighbor.Contract.Abstractions.Shared;
using static Neighbor.Contract.Services.Categories.Response;

namespace Neighbor.Application.Tests.UseCases.V1.Queries.Admin
{
    public class GetDashboardQueryHandlerTests
    {
        private readonly Mock<IDPUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly GetDashboardQueryHandler _handler;

        public GetDashboardQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IDPUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _handler = new GetDashboardQueryHandler(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenDataIsValid()
        {
            // Arrange
            var query = new Query.GetDashboardQuery(2024);

            _mockUnitOfWork.Setup(u => u.OrderRepositories.GetTotalRevenue()).ReturnsAsync(5000.0);
            _mockUnitOfWork.Setup(u => u.AccountRepositories.CountAllUsers()).ReturnsAsync(100);
            _mockUnitOfWork.Setup(u => u.OrderRepositories.CountAmountInYear(It.IsAny<int>()))
                           .ReturnsAsync(new Dictionary<int, double>
                           {
                               { 1, 1000.0 },
                               { 2, 2000.0 },
                               { 3, 1500.0 },
                               { 4, 1200.0 },
                               { 5, 1800.0 },
                               { 6, 2500.0 },
                               { 7, 1300.0 },
                               { 8, 1600.0 },
                               { 9, 1700.0 },
                               { 10, 1900.0 },
                               { 11, 2200.0 },
                               { 12, 2500.0 }
                           });

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            var successResponse = result as Result<Success<Response.DashboardResponse>>;
            Assert.Equal(MessagesList.GetDashboardForTotalSuccess.GetMessage().Code, successResponse.Value.Code);
            Assert.Equal(MessagesList.GetDashboardForTotalSuccess.GetMessage().Message, successResponse.Value.Message);
            Assert.Equal(100, successResponse.Value.Data.TotalUsers);
            Assert.Equal(5000.0, successResponse.Value.Data.TotalRevenue);
            Assert.Equal(12, successResponse.Value.Data.ListMonths.Count);
            Assert.Equal(new List<string>
            {
                "January", "February", "March", "April", "May", "June",
                "July", "August", "September", "October", "November", "December"
            }, successResponse.Value.Data.ListMonths);

            Assert.Equal(12, successResponse.Value.Data.ListRevenueInYear.Count);
            Assert.Equal(1000.0, successResponse.Value.Data.ListRevenueInYear[0]);
            Assert.Equal(2500.0, successResponse.Value.Data.ListRevenueInYear[11]);
        }

        [Fact]
        public async Task Handle_ShouldReturnZeroRevenue_WhenRevenueIsZero()
        {
            // Arrange
            var query = new Query.GetDashboardQuery(2024);

            _mockUnitOfWork.Setup(u => u.OrderRepositories.GetTotalRevenue()).ReturnsAsync(0.0);
            _mockUnitOfWork.Setup(u => u.AccountRepositories.CountAllUsers()).ReturnsAsync(100);
            _mockUnitOfWork.Setup(u => u.OrderRepositories.CountAmountInYear(It.IsAny<int>()))
                           .ReturnsAsync(new Dictionary<int, double>
                           {
                               { 1, 0.0 },
                               { 2, 0.0 },
                               { 3, 0.0 },
                               { 4, 0.0 },
                               { 5, 0.0 },
                               { 6, 0.0 },
                               { 7, 0.0 },
                               { 8, 0.0 },
                               { 9, 0.0 },
                               { 10, 0.0 },
                               { 11, 0.0 },
                               { 12, 0.0 }
                           });

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            var successResponse = result as Result<Success<Response.DashboardResponse>>;
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(0.0, successResponse.Value.Data.TotalRevenue);
        }

        [Fact]
        public async Task Handle_ShouldReturnZeroUsers_WhenNoUsersExist()
        {
            // Arrange
            var query = new Query.GetDashboardQuery(2024);

            _mockUnitOfWork.Setup(u => u.OrderRepositories.GetTotalRevenue()).ReturnsAsync(5000.0);
            _mockUnitOfWork.Setup(u => u.AccountRepositories.CountAllUsers()).ReturnsAsync(0);
            _mockUnitOfWork.Setup(u => u.OrderRepositories.CountAmountInYear(It.IsAny<int>()))
                           .ReturnsAsync(new Dictionary<int, double>
                           {
                               { 1, 1000.0 },
                               { 2, 2000.0 },
                               { 3, 1500.0 },
                               { 4, 1200.0 },
                               { 5, 1800.0 },
                               { 6, 2500.0 },
                               { 7, 1300.0 },
                               { 8, 1600.0 },
                               { 9, 1700.0 },
                               { 10, 1900.0 },
                               { 11, 2200.0 },
                               { 12, 2500.0 }
                           });

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            var successResponse = result as Result<Success<Response.DashboardResponse>>;
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(0, successResponse.Value.Data.TotalUsers);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyRevenue_WhenRevenueInYearIsEmpty()
        {
            // Arrange
            var query = new Query.GetDashboardQuery(2024);

            _mockUnitOfWork.Setup(u => u.OrderRepositories.GetTotalRevenue()).ReturnsAsync(5000.0);
            _mockUnitOfWork.Setup(u => u.AccountRepositories.CountAllUsers()).ReturnsAsync(100);
            _mockUnitOfWork.Setup(u => u.OrderRepositories.CountAmountInYear(It.IsAny<int>()))
                           .ReturnsAsync(new Dictionary<int, double>());  // Empty revenue data

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            var successResponse = result as Result<Success<Response.DashboardResponse>>;
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Empty(successResponse.Value.Data.ListRevenueInYear);
        }
    }
}
