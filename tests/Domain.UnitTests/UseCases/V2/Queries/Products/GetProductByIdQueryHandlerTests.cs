using Moq;
using Neighbor.Application.UseCases.V2.Queries.Products;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.DTOs.ProductDTOs;
using Neighbor.Contract.Services.Products;
using Neighbor.Domain.Abstraction.Dappers;
using Neighbor.Domain.Exceptions;
using Xunit;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Neighbor.Domain.Entities;
using static System.Net.Mime.MediaTypeNames;
using Neighbor.Contract.Enumarations.Product;
using Microsoft.CodeAnalysis;
using Microsoft.Identity.Client;

namespace Neighbor.Application.UnitTests.UseCases.V2.Queries.Products
{
    public class GetProductByIdQueryHandlerTests
    {
        private readonly Mock<IDPUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly GetProductByIdQueryHandler _handler;

        public GetProductByIdQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IDPUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _handler = new GetProductByIdQueryHandler(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var accountId = Guid.NewGuid();
            var query = new Query.GetProductByIdQuery(productId, accountId);

            // Using the static CreateProduct method to create the product
            var product = Product.CreateProductForGetDetailsTest(
                        id: productId,
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
                            Insurance.CreateInsuranceForTestGetDetailsProduct("Test Insurance", DateTime.UtcNow, DateTime.UtcNow.AddYears(1), productId, new List<Images>()
                            {
                                Images.CreateImageForTest("Test")
                            })
                        },
                        productSurcharges: new List<ProductSurcharge>
                        {
                            ProductSurcharge.CreateProductSurchargeForTestGetDetailsProduct(100, productId, Guid.NewGuid(), Surcharge.CreateSurchargeForTest(Guid.NewGuid(), "Test Surcharge Name", "Test Surcharge Description"))
                        },
                        lessor: Lessor.CreateLessorForTest("Test Shop", "Address 1"),
                        category: Category.CreateCategoryForTest(1, "Category 1", false, null),
                        images: new List<Images>()
                        {
                            Images.CreateImageForTest("Test")
                        }
                    );

            _mockUnitOfWork.Setup(u => u.ProductRepositories.GetDetailsAsync(productId)).ReturnsAsync(product);
            _mockUnitOfWork.Setup(u => u.WishlistRepositories.IsProductExistInWishlist(accountId, productId)).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            var productResponse = result.Value.Data;
            Assert.Equal(productId, productResponse.Id);
            Assert.Equal("Test Product", productResponse.Name);
            Assert.True(productResponse.IsAddedToWishlist);
            Assert.Equal("Test Shop", productResponse.Lessor.ShopName);
            Assert.NotNull(productResponse.Category);
            Assert.Equal(1, productResponse.Category.CategoryId);
            Assert.NotNull(productResponse.Insurance);
            Assert.Equal("Test Insurance", productResponse.Insurance.Name);
        }

        [Fact]
        public async Task Handle_ShouldThrowProductNotFoundException_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var accountId = Guid.NewGuid();
            var query = new Query.GetProductByIdQuery(productId, accountId);

            _mockUnitOfWork.Setup(u => u.ProductRepositories.GetDetailsAsync(productId)).ReturnsAsync((Domain.Entities.Product)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ProductException.ProductNotFoundException>(() => _handler.Handle(query, CancellationToken.None));
            Assert.Equal("Can not found this Product!", exception.Message); // Customize the message based on the exception definition
        }

        [Fact]
        public async Task Handle_ShouldReturnProduct_WhenAccountIdIsNull()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var query = new Query.GetProductByIdQuery(productId, null);

            // Using the static CreateProduct method to create the product
            var product = Product.CreateProductForGetDetailsTest(
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
                            Insurance.CreateInsuranceForTestGetDetailsProduct("Test Insurance", DateTime.UtcNow, DateTime.UtcNow.AddYears(1), productId, new List<Images>()
                            {
                                Images.CreateImageForTest("Test")
                            })
                        },
                        productSurcharges: new List<ProductSurcharge>
                        {
                            ProductSurcharge.CreateProductSurchargeForTestGetDetailsProduct(100, productId, Guid.NewGuid(), Surcharge.CreateSurchargeForTest(Guid.NewGuid(), "Test Surcharge Name", "Test Surcharge Description"))
                        },
                        lessor: Lessor.CreateLessorForTest("Shop 1", "Address 1"),
                        category: Category.CreateCategoryForTest(1, "Category 1", false, null),
                        images: new List<Images>()
                        {
                            Images.CreateImageForTest("Test")
                        }
                    );

            _mockUnitOfWork.Setup(u => u.ProductRepositories.GetDetailsAsync(productId)).ReturnsAsync(product);


            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            var productResponse = result.Value.Data;
            Assert.Equal("Test Product", productResponse.Name);
            Assert.False(productResponse.IsAddedToWishlist);
        }
    }
}
