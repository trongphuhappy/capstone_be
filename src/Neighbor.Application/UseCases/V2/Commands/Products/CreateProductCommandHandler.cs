using MediatR;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.DTOs.ProductDTOs;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Contract.Services.Products;
using Neighbor.Domain.Abstraction.Dappers;
using Neighbor.Domain.Abstraction.EntitiyFramework;
using Neighbor.Domain.Entities;
using Neighbor.Domain.Exceptions;
using System.Text.Json;
using static Neighbor.Contract.Services.Categories.Filter;

namespace Neighbor.Application.UseCases.V2.Commands.Products;

public sealed class CreateProductCommandHandler : ICommandHandler<Command.CreateProductCommand>
{
    private readonly IEFUnitOfWork _efUnitOfWork;
    private readonly IMediaService _mediaService;

    public CreateProductCommandHandler(
        IEFUnitOfWork efUnitOfWork,
        IPublisher publisher, IMediaService mediaService)
    {
        _efUnitOfWork = efUnitOfWork;
        _mediaService = mediaService;
    }

    public async Task<Result> Handle(Command.CreateProductCommand request, CancellationToken cancellationToken)
    {
        //Check if Category is existed or not
        var selectColumn = new[] { "Id", "Name", "IsVehicle" };
        var categoryFound = await _efUnitOfWork.CategoryRepository.FindByIdAsync(request.CategoryId);
        if (categoryFound == null)
        {
            throw new CategoryException.CategoryNotFoundException();
        }
        //Check if category is type of vehicle then insurance must exist and all fields must not be null
        else if (categoryFound.IsVehicle && (request.Insurance.Name == null || request.Insurance.Description == null || request.Insurance.IssueDate == null || request.Insurance.ExpirationDate == null || request.Insurance.InsuranceImages == null))
        {
            throw new CategoryException.CategoryMissingInsuranceException();
        }
        //Check if user is a lessor or not
        var lessor = await _efUnitOfWork.LessorRepository.FindSingleAsync(x => x.AccountId == request.UserId);
        if (lessor == null)
        {
            throw new LessorException.LessorNotFoundException();
        }
        //Add Product to Database
        var productCreated = Product.CreateProduct(request.Name, request.Policies, request.Description, request.Price, request.Value, request.CategoryId, lessor.Id);
        _efUnitOfWork.ProductRepository.Add(productCreated);
        await _efUnitOfWork.SaveChangesAsync(cancellationToken);
        //Upload Product Images
        var uploadProductImages = await _mediaService.UploadImagesAsync(request.ProductImages);
        var imageProducts = uploadProductImages.Select(image => new Images(image.ImageUrl, image.PublicImageId, productCreated.Id, null, null));
        _efUnitOfWork.ImagesRepository.AddRange(imageProducts.ToList());
        await _efUnitOfWork.SaveChangesAsync(cancellationToken);
        //Add Insurance if category is type of vehicle
        if (categoryFound.IsVehicle && request.Insurance != null)
        {
            var insuranceCreated = Insurance.CreateInsurance(request.Insurance.Name, request.Insurance.Description, request.Insurance.IssueDate.Value, request.Insurance.ExpirationDate.Value, productCreated.Id);
            _efUnitOfWork.InsuranceRepository.Add(insuranceCreated);
            await _efUnitOfWork.SaveChangesAsync(cancellationToken);
            //Upload Insurance Images
            var uploadInsuranceImages = await _mediaService.UploadImagesAsync(request.Insurance.InsuranceImages);
            var imageInsurances = uploadInsuranceImages.Select(image => new Images(image.ImageUrl, image.PublicImageId, null, insuranceCreated.Id, null));
            _efUnitOfWork.ImagesRepository.AddRange(imageInsurances.ToList());
            await _efUnitOfWork.SaveChangesAsync(cancellationToken);
        }
        //If Surcharge exist then Add new ProductSurcharge based on ProductId, SurchargeId and Price
        if (request.Surcharge.SurchargeId != null && request.Surcharge.Price != null)
        {
            var surchargeFound = await _efUnitOfWork.SurchargeRepository.FindByIdAsync(request.Surcharge.SurchargeId);
            if (surchargeFound == null)
            {
                throw new SurchargeException.SurchargeNotFoundException();
            }
            var productSurcharge = ProductSurcharge.CreateProductSurcharge(request.Surcharge.Price, productCreated.Id, surchargeFound.Id);

            _efUnitOfWork.ProductSurchargeRepository.Add(productSurcharge);
            await _efUnitOfWork.SaveChangesAsync(cancellationToken);
        }
        return Result.Success(new Success(MessagesList.ProductCreateSuccess.GetMessage().Code, MessagesList.ProductCreateSuccess.GetMessage().Message));
    }
}
