using Microsoft.AspNetCore.Http;
using Moq;
using Neighbor.Application.UseCases.V2.Commands.Products;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.DTOs.MediaDTOs;
using Neighbor.Contract.DTOs.ProductDTOs;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Contract.Services.Products;
using Neighbor.Domain.Abstraction.EntitiyFramework;
using Neighbor.Domain.Entities;
using Neighbor.Domain.Exceptions;
using System.Linq.Expressions;
namespace Neighbor.Application.UnitTests.UseCases.V2.Commands.Products;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IEFUnitOfWork> _mockEFUnitOfWork;
    private readonly Mock<IMediaService> _mockMediaService;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _mockEFUnitOfWork = new Mock<IEFUnitOfWork>();
        _mockMediaService = new Mock<IMediaService>();
        _handler = new CreateProductCommandHandler(_mockEFUnitOfWork.Object, _mockMediaService.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowSurchargeNotFoundException_WhenSurchargeDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var lessorId = Guid.NewGuid();
        var command = new Neighbor.Contract.Services.Products.Command.CreateProductCommand(
            Name: "Valid Product",
            Description: "Product Description",
            Value: 200,
            Price: 100,
            MaximumRentDays: 10,
            Policies: "Policy",
            CategoryId: 1,
            UserId: userId,
            ProductImages: new List<IFormFile>(), 
            Insurance: null, 
            ListSurcharges: new List<SurchargeDTO.SurchargeRequestDTO>
            {
            new SurchargeDTO.SurchargeRequestDTO { SurchargeId = Guid.NewGuid(), Price = 50 }
            }
        );

        // Mock dependencies
        _mockEFUnitOfWork.Setup(x => x.CategoryRepository.FindByIdAsync(
            command.CategoryId,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Category, object>>[]>()
        )).ReturnsAsync(new Category()); // Mock Category repository to return a valid Category

        // Mock the LessorRepository to return a valid Lessor (or any necessary behavior)
        _mockEFUnitOfWork.Setup(x => x.LessorRepository.FindSingleAsync(
            It.IsAny<Expression<Func<Lessor, bool>>>(),  // Match any predicate
            It.IsAny<CancellationToken>(),  // Ignore cancellation token
            It.IsAny<Expression<Func<Lessor, object>>[]>()  // Optional includeProperties (can be empty)
        )).ReturnsAsync(new Lessor());  // Mock Lessor repository to return a valid Lessor

        // Mock the ProductRepository if required
        _mockEFUnitOfWork.Setup(x => x.ProductRepository.Add(It.IsAny<Product>())).Verifiable();

        // Mock the ImagesRepository if required
        _mockEFUnitOfWork.Setup(x => x.ImagesRepository.AddRange(It.IsAny<List<Images>>())).Verifiable();

        // Mock the SurchargeRepository to simulate surcharge not found
        _mockEFUnitOfWork.Setup(x => x.SurchargeRepository.FindByIdAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Surcharge, object>>[]>()
        )).ReturnsAsync((Surcharge?)null);  // Simulate the surcharge not being found

        // Mock _mediaService.UploadImagesAsync to return a list of images
        _mockMediaService.Setup(m => m.UploadImagesAsync(It.IsAny<List<IFormFile>>()))
            .ReturnsAsync(new List<ImageDTO>
            {
        new ImageDTO { ImageUrl = "https://example.com/image1.jpg", PublicImageId = "" },
        new ImageDTO { ImageUrl = "https://example.com/image2.jpg", PublicImageId = "" }
            });


        // Act & Assert
        await Assert.ThrowsAsync<SurchargeException.SurchargeNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );
    }

    [Fact]
    public async Task Handle_ShouldCreateProductWithSurcharges_WhenValidRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var lessorId = Guid.NewGuid();
        var command = new Neighbor.Contract.Services.Products.Command.CreateProductCommand(
            Name: "Valid Product",
            Description: "Product Description",
            Value: 200,
            Price: 100,
            MaximumRentDays: 10,
            Policies: "Policy",
            CategoryId: 1,
            UserId: userId,
            ProductImages: new List<IFormFile>(), // Assuming no images for simplicity
            Insurance: null, // No insurance for non-vehicle category
            ListSurcharges: new List<SurchargeDTO.SurchargeRequestDTO>
            {
            new SurchargeDTO.SurchargeRequestDTO { SurchargeId = Guid.NewGuid(), Price = 50 }
            }
        );

        // Mock dependencies
        _mockEFUnitOfWork.Setup(x => x.CategoryRepository.FindByIdAsync(
            command.CategoryId,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Category, object>>[]>()
        )).ReturnsAsync(new Category()); // Mock Category repository to return a valid Category

        _mockEFUnitOfWork.Setup(x => x.LessorRepository.FindSingleAsync(
            It.IsAny<Expression<Func<Lessor, bool>>>(),  // Match any predicate
            It.IsAny<CancellationToken>(),  // Ignore cancellation token
            It.IsAny<Expression<Func<Lessor, object>>[]>()  // Optional includeProperties (can be empty)
        )).ReturnsAsync(new Lessor());  // Mock Lessor repository to return a valid Lessor

        // Mock the ProductRepository to return a valid Product (or any necessary behavior)
        _mockEFUnitOfWork.Setup(x => x.ProductRepository.Add(It.IsAny<Product>())).Verifiable();

        // Mock the ImagesRepository if required
        _mockEFUnitOfWork.Setup(x => x.ImagesRepository.AddRange(It.IsAny<List<Images>>())).Verifiable();

        // Mock the SurchargeRepository to simulate surcharge not found
        _mockEFUnitOfWork.Setup(x => x.SurchargeRepository.FindByIdAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Surcharge, object>>[]>()
        )).ReturnsAsync(new Surcharge());  // Simulate the surcharge not being found

        // Mock _mediaService.UploadImagesAsync to return a list of images
        _mockMediaService.Setup(m => m.UploadImagesAsync(It.IsAny<List<IFormFile>>()))
            .ReturnsAsync(new List<ImageDTO>
            {
        new ImageDTO { ImageUrl = "https://example.com/image1.jpg", PublicImageId = "" },
        new ImageDTO { ImageUrl = "https://example.com/image2.jpg", PublicImageId = "" }
            });

        //Mock ProductSurchargeRepository to return valid ListProductSurcharge
        _mockEFUnitOfWork.Setup(x => x.ProductSurchargeRepository.AddRange(It.IsAny<List<ProductSurcharge>>()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);

        _mockEFUnitOfWork.Verify(x => x.ProductSurchargeRepository.AddRange(It.IsAny<List<ProductSurcharge>>()), Times.Once);
        _mockEFUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }





}
