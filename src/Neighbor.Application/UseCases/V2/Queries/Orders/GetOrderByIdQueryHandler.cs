using AutoMapper;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.DTOs.OrderDTOs;
using Neighbor.Contract.DTOs.ProductDTOs;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Contract.Services.Orders;
using Neighbor.Domain.Abstraction.Dappers;
using Neighbor.Domain.Entities;
using Neighbor.Domain.Exceptions;
using static Neighbor.Contract.DTOs.ProductDTOs.InsuranceDTO;
using static Neighbor.Contract.DTOs.ProductDTOs.ProductDTO;
using static Neighbor.Contract.DTOs.ProductDTOs.SurchargeDTO;
using static Neighbor.Contract.Services.Orders.Response;

namespace Neighbor.Application.UseCases.V2.Queries.Orders;
public sealed class GetOrderByIdQueryHandler : IQueryHandler<Query.GetOrderByIdQuery, Success<OrderResponse>>
{
    private readonly IDPUnitOfWork _dpUnitOfWork;
    private readonly IMapper _mapper;

    public GetOrderByIdQueryHandler(IDPUnitOfWork dpUnitOfWork, IMapper mapper)
    {
        _dpUnitOfWork = dpUnitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<Success<OrderResponse>>> Handle(Query.GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        //Found Order
        var orderFound = await _dpUnitOfWork.OrderRepositories.GetDetailsAsync(request.Id);
        if(orderFound == null)
        {
            throw new OrderException.OrderNotFoundException();
        }
        //Mapping Product to Result
        //Mapping Category of Product of Order
        var category = new CategoryDTO()
        {
            CategoryId = orderFound.Product.Category.Id,
            CategoryName = orderFound.Product.Category.Name,
            IsVehicle = orderFound.Product.Category.IsVehicle
        };
        //Mapping ListImagesUrl of Product of Order
        var listImagesUrl = orderFound.Product.Images.Select(x => x.ImageLink).ToList();
        //Mapping Insurance of Product and check if List Insurance is empty
        var insurance = orderFound.Product.Insurances != null && orderFound.Product.Insurances.Count != 0 && orderFound.Product.Insurances[0].Name != null ? new InsuranceResponseDTO()
        {
            Name = orderFound.Product.Insurances[0].Name,
            IssueDate = orderFound.Product.Insurances[0].IssueDate,
            ExpirationDate = orderFound.Product.Insurances[0].ExpirationDate,
            InsuranceImagesUrl = orderFound.Product.Insurances[0].Images.ToList().Select(image => image.ImageLink).ToList()
        } : null;
        //Mapping Surcharges of Product and check if List Surcharges is empty
        List<SurchargeResponseDTO> surcharges = null;
        if (orderFound.Product.ProductSurcharges != null && orderFound.Product.ProductSurcharges.Count != 0 && orderFound.Product.ProductSurcharges[0].Surcharge.Name != null)
        {
            surcharges = new List<SurchargeResponseDTO>();
            orderFound.Product.ProductSurcharges.ForEach(productSurcharge =>
            {
                surcharges.Add(new SurchargeResponseDTO()
                {
                    SurchargeId = productSurcharge.SurchargeId,
                    Price = productSurcharge.Price,
                    SurchargeName = productSurcharge.Surcharge.Name
                });
            });
        }
        //Mapping Product
        var productDTO = new ProductResponseDTO()
        {
            Id = orderFound.Product.Id,
            Name = orderFound.Product.Name,
            Description = orderFound.Product.Description,
            Value = orderFound.Product.Value,
            Price = orderFound.Product.Price,
            Rating = orderFound.Product.Rating,
            MaximumRentDays = orderFound.Product.MaximumRentDays,
            Policies = orderFound.Product.Policies,
            CreatedDate = orderFound.Product.CreatedDate.Value,
            Category = category,
            ProductImagesUrl = listImagesUrl,
            Insurance = insurance,
            Surcharges = surcharges
        };
        //Mapping User
        var user = new UserDTO()
        {
            UserId = orderFound.Account.Id,
            Email = orderFound.Account.Email,
            FirstName = orderFound.Account.FirstName,
            LastName = orderFound.Account.LastName,
            PhoneNumber = orderFound.Account.PhoneNumber,
            CropAvatarLink = orderFound.Account.CropAvatarUrl,
            FullAvatarLink = orderFound.Account.FullAvatarUrl,
        };
        var lessor = new LessorDTO()
        {
            LessorId = orderFound.Product.Lessor.Id,
            ShopName = orderFound.Product.Lessor.ShopName,
            WareHouseAddress = orderFound.Product.Lessor.WareHouseAddress
        };
        var result = new OrderResponse(orderFound.Id, orderFound.RentTime, orderFound.ReturnTime, orderFound.DeliveryAddress, orderFound.OrderValue, orderFound.OrderStatus, orderFound.OrderReportStatus, orderFound.UserReasonReject, orderFound.LessorReasonReject, orderFound.UserReport, orderFound.AdminReasonReject, orderFound.CreatedDate.Value, productDTO, user, lessor);
        return Result.Success(new Success<OrderResponse>(MessagesList.OrderGetDetailsSuccessfully.GetMessage().Code, MessagesList.OrderGetDetailsSuccessfully.GetMessage().Message, result));
    }
}
