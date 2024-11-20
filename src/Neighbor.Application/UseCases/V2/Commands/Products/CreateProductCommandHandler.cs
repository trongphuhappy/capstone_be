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
    private readonly IDPUnitOfWork _dpUnitOfWork;
    private readonly IPublisher _publisher;
    private readonly IMediaService _mediaService;

    public CreateProductCommandHandler(
        IEFUnitOfWork efUnitOfWork,
        IDPUnitOfWork dpUnitOfWork,
        IPublisher publisher, IMediaService mediaService)
    {
        _efUnitOfWork = efUnitOfWork;
        _dpUnitOfWork = dpUnitOfWork;
        _publisher = publisher;
        _mediaService = mediaService;
    }

    public async Task<Result> Handle(Command.CreateProductCommand request, CancellationToken cancellationToken)
    {
        //Check if Category is existed or not
        var selectColumn = new[] { "Id", "Name", "IsVehicle"};
        var categoryFound = await _dpUnitOfWork.CategoryRepositories.GetPagedAsync(1, 1, new CategoryFilter(request.CategoryId, null, null), selectColumn);
        if (categoryFound == null)
        {
            throw new CategoryException.CategoryNotFoundException();
        }
        //Check if category is type of vehicle then insurance must exist
        else if(categoryFound.Items[0].IsVehicle && request.Insurance == null)
        {
            throw new CategoryException.CategoryMissingInsuranceException();
        }
        //Check if user is a lessor or not
        var lessor = await _dpUnitOfWork.LessorRepositories.GetLessorByUserId(request.UserId);
        if(lessor == null)
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
        _efUnitOfWork.ImagesRepository.AddRange((List<Images>)imageProducts);
        await _efUnitOfWork.SaveChangesAsync(cancellationToken);
        //Add Insurance if category is type of vehicle
        if (categoryFound.Items[0].IsVehicle && request.Insurance == null)
        {
            var insuranceCreated = Insurance.CreateInsurance(request.Insurance.Name, request.Insurance.Description, request.Insurance.IssueDate.Value, request.Insurance.ExpirationDate.Value, productCreated.Id);
            _efUnitOfWork.InsuranceRepository.Add(insuranceCreated);
            await _efUnitOfWork.SaveChangesAsync(cancellationToken);
            //Upload Insurance Images
            var uploadInsuranceImages = await _mediaService.UploadImagesAsync(request.Insurance.InsuranceImages);
            var imageInsurances = uploadInsuranceImages.Select(image => new Images(image.ImageUrl, image.PublicImageId, null, insuranceCreated.Id, null));
            _efUnitOfWork.ImagesRepository.AddRange((List<Images>)imageInsurances);
        }
        //Check if ListSurcharges not equal null then check item in ListSurcharges exist or not
        //If Surcharge exist then Add new ProductSurcharge based on ProductId, SurchargeId and Price
        if (request.Surcharges != null)
        {
            List<ProductSurcharge> productSurcharges = new List<ProductSurcharge>();
            request.Surcharges.ForEach(async surcharge =>
            {
                var surchargeFound = await _efUnitOfWork.SurchargeRepository.FindByIdAsync(surcharge.SurchargeId);
                if(surchargeFound == null)
                {
                    throw new SurchargeException.SurchargeNotFoundException();
                }
                productSurcharges.Add(new ProductSurcharge(surcharge.Price, productCreated.Id, surcharge.SurchargeId));
            });
            _efUnitOfWork.ProductSurchargeRepository.AddRange(productSurcharges);
            await _efUnitOfWork.SaveChangesAsync(cancellationToken);
        }
        return Result.Success(new Success(MessagesList.ProductCreateSuccessfully.GetMessage().Code, MessagesList.ProductCreateSuccessfully.GetMessage().Message));
    }
}
