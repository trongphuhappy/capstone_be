using AutoMapper;
using Moq;
using Neighbor.Application.UseCases.V2.Queries.Orders;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.DTOs.OrderDTOs;
using Neighbor.Contract.DTOs.ProductDTOs;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Contract.Enumarations.Order;
using Neighbor.Contract.Enumarations.Product;
using Neighbor.Contract.Services.Orders;
using Neighbor.Domain.Abstraction.Dappers;
using Neighbor.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static Neighbor.Contract.DTOs.ProductDTOs.ProductDTO;
using static Neighbor.Contract.Services.Orders.Response;

namespace Neighbor.Application.UnitTests.UseCases.V2.Queries.Orders
{
    public class GetAllOrdersQueryHandlerTests
    {
        private readonly Mock<IDPUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly GetAllOrdersQueryHandler _handler;

        public GetAllOrdersQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IDPUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _handler = new GetAllOrdersQueryHandler(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_WhenOrdersAreFound_ReturnsSuccess()
        {
            // Arrange
            var query = new Query.GetAllOrdersQuery
            (
                PageIndex: 1,
                PageSize: 10,
                FilterParams: null,
                SelectedColumns: null
            );

            var orders = new PagedResult<Order>
            (
                items: new List<Order>
                {
                    Order.CreateOrderForTestGetOrder
                    (
                        orderId: Guid.NewGuid(),
                        rentTime: DateTime.UtcNow.AddDays(-2),
                        returnTime: DateTime.UtcNow.AddDays(5),
                        deliveryAddress: "Address 1",
                        orderValue: 500,
                        orderStatus: OrderStatusType.UserApproved,
                        orderReportStatusType: OrderReportStatusType.NotConflict,
                        userReasonReject: null,
                        lessorReasonReject: null,
                        userReport: null,
                        adminReasonReject: null,
                        createdDate: DateTime.UtcNow.AddDays(-10),
                        product: Product.CreateProductForGetAllTest(
                            id: Guid.NewGuid(),
                            name: "Product 2",
                            statusType: StatusType.Available,
                            policies: "Policy 2",
                            description: "Description 2",
                            rating: 3.5,
                            price: 200,
                            value: 700,
                            maximumRentDays: 15,
                            confirmStatus: ConfirmStatus.Pending,
                            createdDate: DateTime.UtcNow.AddDays(-10),
                            wishlists: new List<Wishlist>(),
                            lessor: Lessor.CreateLessorForTest("Shop 2", "Address 2"),
                            category: Category.CreateCategoryForTest(2, "Category 2", true, null),
                            images: new List<Images>()
                            {
                                Images.CreateImageForTest("Test")
                            }
                        ),
                        user: Account.CreateAccountForTestGetOrder(
                        
                            Id: Guid.NewGuid(),
                            FirstName: "John",
                            LastName: "Doe",
                            Email: "john.doe@example.com",
                            PhoneNumber: "1234567890",
                            CropAvatarUrl: "avatar.jpg",
                            FullAvatarUrl: "full-avatar.jpg"
                        )
                    )
                },
                totalCount: 1,
                pageIndex: 1,
                pageSize: 10,
                totalPages: 1
            );

            _mockUnitOfWork.Setup(u => u.OrderRepositories.GetPagedAsync(query.PageIndex, query.PageSize, query.FilterParams, query.SelectedColumns))
                .ReturnsAsync(orders);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            var successResponse = result as Result<Success<PagedResult<OrderResponse>>>;
            Assert.Equal(MessagesList.OrderGetAllSuccessfully.GetMessage().Code, result.Value?.Code);
            Assert.Equal(MessagesList.OrderGetAllSuccessfully.GetMessage().Message, result.Value?.Message);
            Assert.NotNull(result.Value?.Data);
            Assert.Equal(1, result.Value?.Data.Items.Count);

            _mockUnitOfWork.Verify(u => u.OrderRepositories.GetPagedAsync(query.PageIndex, query.PageSize, query.FilterParams, query.SelectedColumns), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenNoOrdersAreFound_ReturnsOrderNotFoundException()
        {
            // Arrange
            var query = new Query.GetAllOrdersQuery
            (
                PageIndex: 1,
                PageSize: 10,
                FilterParams: null,
                SelectedColumns: null
            );

            var orders = new PagedResult<Order>(
                items: new List<Order>(), // No orders
                pageIndex: 1,
                pageSize: 10,
                totalCount: 0,
                totalPages: 0
            );

            _mockUnitOfWork.Setup(u => u.OrderRepositories.GetPagedAsync(query.PageIndex, query.PageSize, query.FilterParams, query.SelectedColumns))
                .ReturnsAsync(orders);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess); // Ensure it signals a failure
            var failureResponse = result as Result<Success<PagedResult<OrderResponse>>>;
            Assert.NotNull(failureResponse);
            Assert.Equal(MessagesList.OrderNotFoundAnyException.GetMessage().Code, result.Value.Code);
            Assert.Equal(MessagesList.OrderNotFoundAnyException.GetMessage().Message, result.Value.Message);

            _mockUnitOfWork.Verify(u => u.OrderRepositories.GetPagedAsync(query.PageIndex, query.PageSize, query.FilterParams, query.SelectedColumns), Times.Once);
        }
    }
}
