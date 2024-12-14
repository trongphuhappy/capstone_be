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
using static Neighbor.Contract.Services.Products.Response;
using static System.Net.Mime.MediaTypeNames;

namespace Neighbor.Application.UnitTests.UseCases.V2.Queries.Products
{
    public class GetAllProductsQueryHandlerTests
    {
        private readonly Mock<IDPUnitOfWork> _mockkUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly GetAllProductsQueryHandler _handler;

        public GetAllProductsQueryHandlerTests()
        {
            _mockkUnitOfWork = new Mock<IDPUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _handler = new GetAllProductsQueryHandler(_mockkUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_WhenProductsAreFound_ReturnsSuccess()
        {
            // Arrange
            var query = new Query.GetAllProductsQuery
            (
                PageIndex: 1,
                PageSize: 10,
                FilterParams: new Filter.ProductFilter(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null), 
                SelectedColumns: null
            );

            var products = new PagedResult<Product>
            (
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
                        wishlists: new List<Wishlist>(),
                        lessor: Lessor.CreateLessorForTest("Shop 1", "Address 1"),
                        category: Category.CreateCategoryForTest(1, "Category 1", false, null),
                        images: new List<Images>()
                        {
                            Images.CreateImageForTest("Test") 
                        }
                    ),
                    Product.CreateProductForGetAllTest(
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
                    )
                },
                totalCount: 2,
                pageIndex: 1,
                pageSize: 10,
                totalPages: 1
            );

            var productResponses = new List<ProductResponse>
            {
                new ProductResponse(
                    Id: Guid.NewGuid(),
                    Name: "Product 1",
                    StatusType: StatusType.Available,
                    Policies: "Policy 1",
                    Description: "Description 1",
                    Rating: 4.5,
                    Price: 100,
                    Value: 500,
                    MaximumRentDays: 10,
                    ConfirmStatus: ConfirmStatus.Approved,
                    CreatedDate: DateTime.UtcNow.AddDays(-5),
                    AddToWishlistDate: null,
                    IsAddedToWishlist: false,
                    IsProductBelongsToUser: false,
                    Category: new CategoryDTO()
                    {
                        CategoryId = 1,
                        CategoryName = "Category 1",
                        IsVehicle = false
                    },
                    ProductImagesUrl: null,
                    Insurance: null,
                    Surcharges: null,
                    Lessor: new LessorDTO()
                    {
                        AccountId = Guid.NewGuid(),
                        ShopName = "Shop 1",
                        WareHouseAddress = "Address 1"
                    },
                    RejectReason: null
                ),
                new ProductResponse(
                    Id: Guid.NewGuid(),
                    Name: "Product 2",
                    StatusType: StatusType.Available,
                    Policies: "Policy 2",
                    Description: "Description 2",
                    Rating: 3.5,
                    Price: 200,
                    Value: 700,
                    MaximumRentDays: 15,
                    ConfirmStatus: ConfirmStatus.Pending,
                    CreatedDate: DateTime.UtcNow.AddDays(-10),
                    AddToWishlistDate: null,
                    IsAddedToWishlist: true,
                    IsProductBelongsToUser: false,
                    Category: new CategoryDTO()
                    {
                        CategoryId = 2,
                        CategoryName = "Category 2",
                        IsVehicle = true
                    },                    
                    ProductImagesUrl: null,
                    Insurance: null,
                    Surcharges: null,
                    Lessor: new LessorDTO()
                    {
                        AccountId = Guid.NewGuid(),
                        ShopName = "Shop 2",
                        WareHouseAddress = "Address 2"
                    },                    
                    RejectReason: null
                )
            };

            var mappedResult = new PagedResult<ProductResponse>
            (
                items: productResponses,
                pageIndex: 1,
                pageSize: 10,
                totalCount: 2,
                totalPages: 1
            );

            _mockkUnitOfWork.Setup(u => u.ProductRepositories.GetPagedAsync(query.PageIndex, query.PageSize, query.FilterParams, query.SelectedColumns))
                .ReturnsAsync(products);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            var successResponse = result as Result<Success<PagedResult<ProductResponse>>>;
            Assert.Equal(MessagesList.ProductGetAllSuccess.GetMessage().Code, result.Value?.Code);
            Assert.Equal(MessagesList.ProductGetAllSuccess.GetMessage().Message, result.Value?.Message);
            Assert.NotNull(result.Value?.Data);
            Assert.Equal(2, result.Value?.Data.Items.Count);

            _mockkUnitOfWork.Verify(u => u.ProductRepositories.GetPagedAsync(query.PageIndex, query.PageSize, query.FilterParams, query.SelectedColumns), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenNoProductsFound_ReturnsProductNotFoundException()
        {
            // Arrange
            var query = new Query.GetAllProductsQuery
            (
                PageIndex: 1,
                PageSize: 10,
                FilterParams: null,
                SelectedColumns: null
            );

            var products = new PagedResult<Product>
            (
                items: new List<Product>(), // No products
                pageIndex: 1,
                pageSize: 10,
                totalCount: 0,
                totalPages: 0
            );

            _mockkUnitOfWork.Setup(u => u.ProductRepositories.GetPagedAsync(query.PageIndex, query.PageSize, query.FilterParams, query.SelectedColumns))
                .ReturnsAsync(products);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess); // Ensure it signals a failure
            var failureResponse = result as Result<Success<PagedResult<ProductResponse>>>;
            Assert.NotNull(failureResponse);
            Assert.Equal(MessagesList.ProductNotFoundAnyException.GetMessage().Code, result.Value.Code);
            Assert.Equal(MessagesList.ProductNotFoundAnyException.GetMessage().Message, result.Value.Message);

            _mockkUnitOfWork.Verify(u => u.ProductRepositories.GetPagedAsync(query.PageIndex, query.PageSize, query.FilterParams, query.SelectedColumns), Times.Once);
        }

    }
}
