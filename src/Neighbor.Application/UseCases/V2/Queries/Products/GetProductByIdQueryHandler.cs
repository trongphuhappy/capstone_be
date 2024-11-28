using AutoMapper;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.DTOs.ProductDTOs;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Contract.Services.Products;
using Neighbor.Domain.Abstraction.Dappers;
using Neighbor.Domain.Exceptions;
using static Neighbor.Contract.DTOs.ProductDTOs.InsuranceDTO;
using static Neighbor.Contract.DTOs.ProductDTOs.SurchargeDTO;
using static Neighbor.Contract.Services.Products.Response;

namespace Neighbor.Application.UseCases.V1.Queries.Products;

public sealed class GetProductByIdQueryHandler : IQueryHandler<Query.GetProductByIdQuery, Success<ProductResponse>>
{
    private readonly IDPUnitOfWork _dpUnitOfWork;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(IDPUnitOfWork dpUnitOfWork, IMapper mapper)
    {
        _dpUnitOfWork = dpUnitOfWork;
        _mapper = mapper;
    }
    public async Task<Result<Success<ProductResponse>>> Handle(Query.GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        //Find Product
        var product = await _dpUnitOfWork.ProductRepositories.GetDetailsAsync(request.Id);
        if (product == null)
        {
            throw new ProductException.ProductNotFoundException();
        }
        //Mapping Product to Result
        //Mapping Insurance of Product
        var insurance = product.Insurances != null && product.Insurances.Count != 0 && product.Insurances[0].Name != null ? new InsuranceResponseDTO()
        {
            Name = product.Insurances[0].Name,
            IssueDate = product.Insurances[0].IssueDate,
            ExpirationDate = product.Insurances[0].ExpirationDate,
            InsuranceImagesUrl = product.Insurances[0].Images.ToList().Select(image => image.ImageLink).ToList()
        } : null;
        //Mapping Surcharges of Product
        List<SurchargeResponseDTO> surcharges = null;
        if (product.ProductSurcharges != null && product.ProductSurcharges.Count != 0 && product.ProductSurcharges[0].Surcharge.Name != null)
        {
            surcharges = new List<SurchargeResponseDTO>();
            product.ProductSurcharges.ForEach(productSurcharge =>
            {
                surcharges.Add(new SurchargeResponseDTO()
                {
                    SurchargeId = productSurcharge.SurchargeId,
                    Price = productSurcharge.Price,
                    SurchargeName = productSurcharge.Surcharge.Name
                });
            });
        }
        //var surcharge = product.ProductSurcharges != null && product.ProductSurcharges[0].Surcharge.Name != null ? new List<SurchargeResponseDTO()
        //{
        //    SurchargeId = product.ProductSurcharges[0].SurchargeId,
        //    Price = product.ProductSurcharges[0].Price,
        //    SurchargeName = product.ProductSurcharges[0].Surcharge.Name
        //} : null;
        //Mapping Lessor of Product
        var lessor = new LessorDTO()
        {
            LessorId = product.LessorId,
            ShopName = product.Lessor.ShopName,
            WareHouseAddress = product.Lessor.WareHouseAddress
        };
        //Mapping Category of Product
        var category = new CategoryDTO()
        {
            CategoryId = product.Category.Id,
            CategoryName = product.Category.Name,
            IsVehicle = product.Category.IsVehicle,
        };
        //Check if GetDetailsProduct By User then Check if Product has existed in Wishlist or not
        if (request.AccountId != null)
        {
            //Check if Product belongs to User or not
            bool isProductBelongsToUser = product.Lessor.AccountId == request.AccountId ? true : false;
            surcharges = new List<SurchargeResponseDTO>();

            //Check if Product has already added to Wishlist
            bool isProductExistInWishlist = await _dpUnitOfWork.WishlistRepositories.IsProductExistInWishlist(request.AccountId.Value, product.Id);
            var result = new ProductResponse(product.Id, product.Name, product.StatusType, product.Policies, product.Description, product.Rating, product.Price, product.Value, product.MaximumRentDays, product.ConfirmStatus, isProductExistInWishlist, isProductBelongsToUser, category, product.Images.ToList().Select(x => x.ImageLink).ToList(), insurance, surcharges, lessor);
            //Return result
            return Result.Success(new Success<ProductResponse>(MessagesList.ProductGetDetailsSuccess.GetMessage().Code, MessagesList.ProductGetDetailsSuccess.GetMessage().Message, result));
        }
        else
        {
            var result = new ProductResponse(product.Id, product.Name, product.StatusType, product.Policies, product.Description, product.Rating, product.Price, product.Value, product.MaximumRentDays, product.ConfirmStatus, false, false, category, product.Images.ToList().Select(x => x.ImageLink).ToList(), insurance, surcharges, lessor);
            //Return result
            return Result.Success(new Success<ProductResponse>(MessagesList.ProductGetDetailsSuccess.GetMessage().Code, MessagesList.ProductGetDetailsSuccess.GetMessage().Message, result));
        }

    }
}
