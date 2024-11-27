using AutoMapper;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.DTOs.ProductDTOs;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Contract.Services.Products;
using Neighbor.Domain.Abstraction.Dappers;
using static Neighbor.Contract.Services.Products.Response;

namespace Neighbor.Application.UseCases.V1.Queries.Products;

public sealed class GetAllProductsInWishlistQueryHandler : IQueryHandler<Query.GetAllProductsInWishlistQuery, Success<PagedResult<ProductResponse>>>
{
    private readonly IDPUnitOfWork _dpUnitOfWork;
    private readonly IMapper _mapper;

    public GetAllProductsInWishlistQueryHandler(IDPUnitOfWork dpUnitOfWork, IMapper mapper)
    {
        _dpUnitOfWork = dpUnitOfWork;
        _mapper = mapper;
    }
    public async Task<Result<Success<PagedResult<ProductResponse>>>> Handle(Query.GetAllProductsInWishlistQuery request, CancellationToken cancellationToken)
    {
        //Find all Products
        var listProducts = await _dpUnitOfWork.ProductRepositories.GetProductsInWishlistAsync(request.AccountId, request.PageIndex, request.PageSize, request.FilterParams, request.SelectedColumns);
        var listProductsDTO = new List<ProductResponse>();
        //Mapping Product to ProductResponse
        for(int i = 0; i < listProducts.Items.Count; i++)
        {
            var product = listProducts.Items[i];
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
            listProductsDTO.Add(new ProductResponse(product.Id, product.Name, product.StatusType, product.Policies, product.Description, product.Rating, product.Price, product.Value, product.MaximumRentDays, product.ConfirmStatus, true, false, category, product.Images.ToList().Select(x => x.ImageLink).ToList(), null, null, lessor));
        }
        //Initial result
        var result = new PagedResult<ProductResponse>(listProductsDTO, listProducts.PageIndex, listProducts.PageSize, listProducts.TotalCount, listProducts.TotalPages);
        //Check if ListCategory is empty
        if (listProducts.Items.Count == 0)
        {
            return Result.Success(new Success<PagedResult<ProductResponse>>(MessagesList.ProductNotFoundAnyException.GetMessage().Code, MessagesList.ProductNotFoundAnyException.GetMessage().Message, result));

        }
        //Return result
        return Result.Success(new Success<PagedResult<ProductResponse>>(MessagesList.ProductGetAllInWishlistSuccess.GetMessage().Code, MessagesList.ProductGetAllInWishlistSuccess.GetMessage().Message, result));
    }
}
