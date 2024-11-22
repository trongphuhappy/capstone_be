//using Moq;
//using Neighbor.Application.UseCases.V2.Commands.Products;
//using Neighbor.Contract.Abstractions.Services;
//using Neighbor.Contract.Abstractions.Shared;
//using Neighbor.Contract.DTOs.ProductDTOs;
//using Neighbor.Contract.Enumarations.MessagesList;
//using Neighbor.Contract.Services.Products;
//using Neighbor.Domain.Abstraction.EntitiyFramework;
//using Neighbor.Domain.Entities;
//using Neighbor.Domain.Exceptions;
//using System.Linq.Expressions;

//public class CreateProductCommandHandlerTests
//{
//    private readonly Mock<IEFUnitOfWork> _mockEFUnitOfWork;
//    private readonly Mock<IMediaService> _mockMediaService;
//    private readonly CreateProductCommandHandler _handler;

//    public CreateProductCommandHandlerTests()
//    {
//        _mockEFUnitOfWork = new Mock<IEFUnitOfWork>();
//        _mockMediaService = new Mock<IMediaService>();
//        _handler = new CreateProductCommandHandler(_mockEFUnitOfWork.Object, _mockMediaService.Object);
//    }

//    [Fact]
//    public async Task Handle_ShouldThrowCategoryNotFoundException_WhenCategoryDoesNotExist()
//    {
//        // Arrange
//        var command = new Neighbor.Contract.Services.Products.Command.CreateProductCommand
//        {
//            CategoryId = 1,
//            Name = "Product Name",
//            Policies = "Policy",
//            Price = 100,
//            Value = 200,
//            Description = "Product Description",
//            UserId = Guid.NewGuid()
//        };

//        _mockEFUnitOfWork.Setup(x => x.CategoryRepository.FindByIdAsync(command.CategoryId))
//                         .ReturnsAsync((Category?)null);

//        // Act & Assert
//        await Assert.ThrowsAsync<CategoryException.CategoryNotFoundException>(() =>
//            _handler.Handle(command, CancellationToken.None)
//        );
//    }

//    [Fact]
//    public async Task Handle_ShouldThrowCategoryMissingInsuranceException_WhenCategoryIsVehicleAndInsuranceIsInvalid()
//    {
//        // Arrange
//        var command = new Neighbor.Contract.Services.Products.Command.CreateProductCommand
//        {
//            CategoryId = 1,
//            Name = "Vehicle",
//            Policies = "Policy",
//            Price = 100,
//            Value = 200,
//            Description = "Vehicle Description",
//            UserId = Guid.NewGuid(),
//            Insurance = new InsuranceDto { Name = null }
//        };

//        _mockEFUnitOfWork.Setup(x => x.CategoryRepository.FindByIdAsync(command.CategoryId))
//                         .ReturnsAsync(new Category { IsVehicle = true });

//        // Act & Assert
//        await Assert.ThrowsAsync<CategoryException.CategoryMissingInsuranceException>(() =>
//            _handler.Handle(command, CancellationToken.None)
//        );
//    }

//    [Fact]
//    public async Task Handle_ShouldThrowLessorNotFoundException_WhenLessorDoesNotExist()
//    {
//        // Arrange
//        var command = new Neighbor.Contract.Services.Products.Command.CreateProductCommand
//        {
//            CategoryId = 1,
//            Name = "Product Name",
//            Policies = "Policy",
//            Price = 100,
//            Value = 200,
//            Description = "Product Description",
//            UserId = Guid.NewGuid()
//        };

//        _mockEFUnitOfWork.Setup(x => x.CategoryRepository.FindByIdAsync(command.CategoryId))
//                         .ReturnsAsync(new Category { IsVehicle = false });

//        _mockEFUnitOfWork.Setup(x => x.LessorRepository.FindSingleAsync(It.IsAny<Expression<Func<Lessor, bool>>>()))
//                         .ReturnsAsync((Lessor?)null);

//        // Act & Assert
//        await Assert.ThrowsAsync<LessorException.LessorNotFoundException>(() =>
//            _handler.Handle(command, CancellationToken.None)
//        );
//    }

//    [Fact]
//    public async Task Handle_ShouldCreateProduct_WhenValidRequest()
//    {
//        // Arrange
//        var command = new Neighbor.Contract.Services.Products.Command.CreateProductCommand
//        {
//            CategoryId = 1,
//            Name = "Valid Product",
//            Policies = "Policy",
//            Price = 100,
//            Value = 200,
//            Description = "Product Description",
//            UserId = Guid.NewGuid(),
//            ProductImages = new List<byte[]> { new byte[] { 0x01 } }
//        };

//        _mockEFUnitOfWork.Setup(x => x.CategoryRepository.FindByIdAsync(command.CategoryId))
//                         .ReturnsAsync(new Category { IsVehicle = false });

//        _mockEFUnitOfWork.Setup(x => x.LessorRepository.FindSingleAsync(It.IsAny<Expression<Func<Lessor, bool>>>()))
//                         .ReturnsAsync(new Lessor { Id = Guid.NewGuid() });

//        _mockMediaService.Setup(x => x.UploadImagesAsync(command.ProductImages))
//                         .ReturnsAsync(new List<MediaResult>
//                         {
//                             new MediaResult { ImageUrl = "http://image.com", PublicImageId = "image-id" }
//                         });

//        // Act
//        var result = await _handler.Handle(command, CancellationToken.None);

//        // Assert
//        Assert.NotNull(result);
//        Assert.True(result.IsSuccess);

//        var successMessage = ((Result<Success>)result).Value;
//        Assert.Equal(MessagesList.ProductCreateSuccess.GetMessage().Message, successMessage.Message);
//        Assert.Equal(MessagesList.ProductCreateSuccess.GetMessage().Code, successMessage.Code);

//        _mockEFUnitOfWork.Verify(x => x.ProductRepository.Add(It.IsAny<Product>()), Times.Once);
//        _mockEFUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
//    }

//    [Fact]
//    public async Task Handle_ShouldAddInsurance_WhenCategoryIsVehicle()
//    {
//        // Arrange
//        var command = new Neighbor.Contract.Services.Products.Command.CreateProductCommand
//        {
//            CategoryId = 1,
//            Name = "Vehicle Product",
//            Policies = "Policy",
//            Price = 100,
//            Value = 200,
//            Description = "Product Description",
//            UserId = Guid.NewGuid(),
//            Insurance = new InsuranceDto
//            {
//                Name = "Insurance Name",
//                IssueDate = DateTime.UtcNow,
//                ExpirationDate = DateTime.UtcNow.AddYears(1),
//                InsuranceImages = new List<byte[]> { new byte[] { 0x01 } }
//            }
//        };

//        _mockEFUnitOfWork.Setup(x => x.CategoryRepository.FindByIdAsync(command.CategoryId))
//                         .ReturnsAsync(new Category { IsVehicle = true });

//        _mockEFUnitOfWork.Setup(x => x.LessorRepository.FindSingleAsync(It.IsAny<Expression<Func<Lessor, bool>>>()))
//                         .ReturnsAsync(new Lessor { Id = Guid.NewGuid() });

//        _mockMediaService.Setup(x => x.UploadImagesAsync(command.Insurance.InsuranceImages))
//                         .ReturnsAsync(new List<MediaResult>
//                         {
//                             new MediaResult { ImageUrl = "http://insurance-image.com", PublicImageId = "insurance-id" }
//                         });

//        // Act
//        var result = await _handler.Handle(command, CancellationToken.None);

//        // Assert
//        Assert.NotNull(result);
//        Assert.True(result.IsSuccess);

//        _mockEFUnitOfWork.Verify(x => x.InsuranceRepository.Add(It.IsAny<Insurance>()), Times.Once);
//        _mockEFUnitOfWork.Verify(x => x.ImagesRepository.AddRange(It.IsAny<List<Images>>()), Times.AtLeastOnce);
//    }
//}
