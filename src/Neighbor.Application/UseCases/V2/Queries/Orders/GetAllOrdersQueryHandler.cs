using AutoMapper;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.DTOs.OrderDTOs;
using Neighbor.Contract.DTOs.ProductDTOs;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Contract.Services.Orders;
using Neighbor.Domain.Abstraction.Dappers;
using static Neighbor.Contract.DTOs.ProductDTOs.ProductDTO;
using static Neighbor.Contract.Services.Orders.Response;

namespace Neighbor.Application.UseCases.V2.Queries.Orders;

public sealed class GetAllOrdersQueryHandler : IQueryHandler<Query.GetAllOrdersQuery, Success<PagedResult<OrderResponse>>>
{
    private readonly IDPUnitOfWork _dpUnitOfWork;
    private readonly IMapper _mapper;

    public GetAllOrdersQueryHandler(IDPUnitOfWork dpUnitOfWork, IMapper mapper)
    {
        _dpUnitOfWork = dpUnitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<Success<PagedResult<OrderResponse>>>> Handle(Query.GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        var listOrders = await _dpUnitOfWork.OrderRepositories.GetPagedAsync(request.PageIndex, request.PageSize, request.FilterParams, request.SelectedColumns);

        var listOrdersDTO = new List<OrderResponse>();

        //Mapping Product to ProductResponse
        listOrders.Items.ForEach(order =>
        {
            var product = new ProductResponseDTO()
            {
                Id = order.Product.Id,
                Name = order.Product.Name,
                Description = order.Product.Description,
                Value = order.Product.Value,
                Price = order.Product.Price,
                Rating = order.Product.Rating,
                MaximumRentDays = order.Product.MaximumRentDays,
                Policies = order.Product.Policies,
                CreatedDate = order.Product.CreatedDate.Value,
                Category = new CategoryDTO()
                {
                    CategoryId = order.Product.Category.Id,
                    CategoryName = order.Product.Category.Name,
                    IsVehicle = order.Product.Category.IsVehicle
                },
                ProductImagesUrl = order.Product.Images.Select(x => x.ImageLink).ToList(),
                Insurance = null,
                Surcharges = null,
            };
            var user = new UserDTO()
            {
                UserId = order.Account.Id,
                FirstName = order.Account.FirstName,
                LastName = order.Account.LastName,
                Email = order.Account.Email,
                PhoneNumber = order.Account.PhoneNumber,
                CropAvatarLink = order.Account.CropAvatarUrl,
                FullAvatarLink = order.Account.FullAvatarUrl
            };
            var lessor = new LessorDTO()
            {
                LessorId = order.Product.Lessor.Id,
                ShopName = order.Product.Lessor.ShopName,
                WareHouseAddress = order.Product.Lessor.WareHouseAddress
            };
            listOrdersDTO.Add(new OrderResponse(order.Id, order.RentTime, order.ReturnTime, order.DeliveryAddress, order.OrderValue, order.OrderStatus, order.OrderReportStatus, order.UserReasonReject, order.LessorReasonReject, order.UserReport, order.AdminReasonReject, order.CreatedDate.Value, product, user, lessor));
        });
        //Initial result
        var result = new PagedResult<OrderResponse>(listOrdersDTO, listOrders.PageIndex, listOrders.PageSize, listOrders.TotalCount, listOrders.TotalPages);
        //Check if ListCategory is empty
        if (listOrders.Items.Count == 0)
        {
            return Result.Success(new Success<PagedResult<OrderResponse>>(MessagesList.OrderNotFoundAnyException.GetMessage().Code, MessagesList.OrderNotFoundAnyException.GetMessage().Message, result));

        }
        //Return result
        return Result.Success(new Success<PagedResult<OrderResponse>>(MessagesList.OrderGetAllSuccessfully.GetMessage().Code, MessagesList.OrderGetAllSuccessfully.GetMessage().Message, result));
    }
}
