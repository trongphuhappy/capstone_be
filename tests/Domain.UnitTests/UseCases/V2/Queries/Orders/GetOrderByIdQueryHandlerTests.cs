using Moq;
using Xunit;
using AutoMapper;
using System;
using System.Threading;
using System.Threading.Tasks;
using Neighbor.Application.UseCases.V2.Queries.Orders;
using Neighbor.Contract.Services.Orders;
using Neighbor.Domain.Entities;
using Neighbor.Domain.Abstraction.Dappers;
using Neighbor.Domain.Exceptions;
using Neighbor.Contract.DTOs.OrderDTOs;
using Neighbor.Contract.DTOs.ProductDTOs;
using Neighbor.Contract.Enumarations.MessagesList;
using System.Collections.Generic;
using System.Linq;
using Neighbor.Contract.Enumarations.Order;
using Neighbor.Contract.Enumarations.Product;
using Microsoft.CodeAnalysis;

namespace Neighbor.Application.UnitTests.UseCases.V2.Queries.Orders
{
    public class GetOrderByIdQueryHandlerTests
    {
        private readonly Mock<IDPUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly GetOrderByIdQueryHandler _handler;

        public GetOrderByIdQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IDPUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _handler = new GetOrderByIdQueryHandler(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_WhenOrderFound_ReturnsSuccess()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var query = new Query.GetOrderByIdQuery(orderId);

            var order = Order.CreateOrderForTestGetOrder
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
                        product: Product.CreateProductForGetDetailsTest(
                            id: Guid.NewGuid(),
                            name: "Test Product",
                            statusType: StatusType.Available,
                            policies: "Policy 1",
                            description: "Description 1",
                            rating: 4.5,
                            price: 100,
                            value: 500,
                            maximumRentDays: 10,
                            confirmStatus: ConfirmStatus.Approved,
                            createdDate: DateTime.UtcNow.AddDays(-5),
                            wishlists: new List<Wishlist>()
                            {
                                Wishlist.CreateWishlistForTest(DateTime.UtcNow)
                            },
                            insurances: new List<Insurance>
                            {
                                Insurance.CreateInsuranceForTestGetDetailsProduct("Test Insurance", DateTime.UtcNow, DateTime.UtcNow.AddYears(1), Guid.NewGuid(), new List<Images>()
                                {
                                    Images.CreateImageForTest("Test")
                                })
                            },
                        productSurcharges: new List<ProductSurcharge>
                        {
                            ProductSurcharge.CreateProductSurchargeForTestGetDetailsProduct(100, Guid.NewGuid(), Guid.NewGuid(), Surcharge.CreateSurchargeForTest(Guid.NewGuid(), "Test Surcharge Name", "Test Surcharge Description"))
                        },
                        lessor: Lessor.CreateLessorForTest("Test Shop", "Address 1"),
                        category: Category.CreateCategoryForTest(1, "Category 1", false, null),
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
                        ));

            _mockUnitOfWork.Setup(u => u.OrderRepositories.GetDetailsAsync(orderId))
                .ReturnsAsync(order);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(MessagesList.OrderGetDetailsSuccessfully.GetMessage().Code, result.Value.Code);
            Assert.Equal(MessagesList.OrderGetDetailsSuccessfully.GetMessage().Message, result.Value.Message);
            Assert.NotNull(result.Value.Data);
            Assert.Equal(order.Id, result.Value.Data.Id);
            Assert.Equal(order.Product.Name, result.Value.Data.Product.Name);
            Assert.Equal(order.Account.FirstName, result.Value.Data.User.FirstName);
        }

        [Fact]
        public async Task Handle_WhenOrderNotFound_ThrowsOrderNotFoundException()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var query = new Query.GetOrderByIdQuery(orderId);

            _mockUnitOfWork.Setup(u => u.OrderRepositories.GetDetailsAsync(orderId))
                .ReturnsAsync((Order)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<OrderException.OrderNotFoundException>(() => _handler.Handle(query, CancellationToken.None));
            Assert.Equal("Can not find this Order", exception.Message); // Assuming the message in exception
        }
    }
}
