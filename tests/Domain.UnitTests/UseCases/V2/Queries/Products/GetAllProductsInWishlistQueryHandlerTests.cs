using AutoMapper;
using Moq;
using Neighbor.Application.UseCases.V2.Queries.Products;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.DTOs.ProductDTOs;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Contract.Enumarations.Product;
using Neighbor.Contract.Services.Products;
using Neighbor.Domain.Abstraction.Dappers;
using Neighbor.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Neighbor.Application.UnitTests.UseCases.V2.Queries.Products
{
    public class GetAllProductsInWishlistQueryHandlerTests
    {
        private readonly Mock<IDPUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly GetAllProductsInWishlistQueryHandler _handler;

        public GetAllProductsInWishlistQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IDPUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _handler = new GetAllProductsInWishlistQueryHandler(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_WhenProductsAreInWishlist_ReturnsSuccess()
        {
            // Arrange
            var query = new Query.GetAllProductsInWishlistQuery(
                AccountId: Guid.NewGuid(),
                PageIndex: 1,
                PageSize: 10,
                FilterParams: null,
                SelectedColumns: null
            );

            var products = new PagedResult<Product>(
                items: new List<Product>
                {
                    Product.CreateProductForGetAllTest(
                        id: Guid.NewGuid(),
                        name: "Product 1",
                        statusType: StatusType.Available,
                        policies: "Policy 1",
                        description: "Description 1",
                        rating: 4.5,
                        price: 100,
                        value: 500,
                        maximumRentDays: 10,
                        confirmStatus: ConfirmStatus.Approved,
                        createdDate: DateTime.UtcNow.AddDays(-5),
                        wishlists: new List<Wishlist>
                        {
                            Wishlist.CreateWishlistForTest(DateTime.UtcNow.AddDays(-1))
                        },
                        lessor: Lessor.CreateLessorForTest("Shop 1", "Address 1"),
                        category: Category.CreateCategoryForTest(1, "Category 1", false, null),
                        images: new List<Images>
                        {
                            Images.CreateImageForTest("TestImage1")
                        }
                    )
                },
                totalCount: 1,
                pageIndex: 1,
                pageSize: 10,
                totalPages: 1
            );

            _mockUnitOfWork
                .Setup(u => u.ProductRepositories.GetProductsInWishlistAsync(query.AccountId, query.PageIndex, query.PageSize, query.FilterParams, query.SelectedColumns))
                .ReturnsAsync(products);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            var successResponse = result.Value;
            Assert.Equal(MessagesList.ProductGetAllInWishlistSuccess.GetMessage().Code, successResponse.Code);
            Assert.Equal(MessagesList.ProductGetAllInWishlistSuccess.GetMessage().Message, successResponse.Message);
            Assert.NotNull(successResponse.Data);
            Assert.Single(successResponse.Data.Items);

            _mockUnitOfWork.Verify(u => u.ProductRepositories.GetProductsInWishlistAsync(query.AccountId, query.PageIndex, query.PageSize, query.FilterParams, query.SelectedColumns), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenNoProductsInWishlist_ReturnsProductNotFoundException()
        {
            // Arrange
            var query = new Query.GetAllProductsInWishlistQuery(
                AccountId: Guid.NewGuid(),
                PageIndex: 1,
                PageSize: 10,
                FilterParams: null,
                SelectedColumns: null
            );

            var products = new PagedResult<Product>(
                items: new List<Product>(), // No products
                totalCount: 0,
                pageIndex: 1,
                pageSize: 10,
                totalPages: 0
            );

            _mockUnitOfWork
                .Setup(u => u.ProductRepositories.GetProductsInWishlistAsync(query.AccountId, query.PageIndex, query.PageSize, query.FilterParams, query.SelectedColumns))
                .ReturnsAsync(products);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            var successResponse = result.Value;
            Assert.Equal(MessagesList.ProductNotFoundAnyException.GetMessage().Code, successResponse.Code);
            Assert.Equal(MessagesList.ProductNotFoundAnyException.GetMessage().Message, successResponse.Message);
            Assert.Empty(successResponse.Data.Items);

            _mockUnitOfWork.Verify(u => u.ProductRepositories.GetProductsInWishlistAsync(query.AccountId, query.PageIndex, query.PageSize, query.FilterParams, query.SelectedColumns), Times.Once);
        }
    }
}
