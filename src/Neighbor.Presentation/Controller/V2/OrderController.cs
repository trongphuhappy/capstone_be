using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.DTOs.ProductDTOs;
using Neighbor.Contract.Services.Orders;
using Neighbor.Presentation.Abstractions;
using System.Security.Claims;
using static Neighbor.Contract.DTOs.ProductDTOs.OrderDTO;
using static Neighbor.Contract.Services.Orders.Filter;

namespace Neighbor.Presentation.Controller.V2;

[ApiVersion(2)]
public class OrderController : ApiController
{
    public OrderController(ISender sender) : base(sender)
    { }

    [Authorize]
    [HttpPost("create_order", Name = "CreateOrder")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    public async Task<IActionResult> CreateOrder([FromBody] OrderDTO.OrderRequestDTO order)
    {
        var userId = Guid.Parse(User.FindFirstValue("UserId"));
        var result = await Sender.Send(new Command.CreateOrderBankingCommand(userId, order.ProductId, order.RentTime, order.ReturnTime));
        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    [HttpGet("order_success", Name = "OrderSuccess")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> OrderSuccess([FromQuery] long orderId)
    {
        var result = await Sender.Send(new Command.OrderSuccessCommand(orderId));
        if (result.IsFailure)
            return HandlerFailure(result);
        
        return Redirect(result.Value.Data.Url);
    }

    [HttpGet("order_fail", Name = "OrderFail")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> OrderFail([FromQuery] long orderId)
    {
        var result = await Sender.Send(new Command.OrderFailCommand(orderId));
        if (result.IsFailure)
            return HandlerFailure(result);
        
        return Redirect(result.Value.Data.Url);
    }

    [Authorize]
    [HttpPut("user_confirm_order", Name = "UserConfirmOrder")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    public async Task<IActionResult> UserConfirmOrder([FromBody] OrderRequestConfirmDTO order)
    {
        var userId = Guid.Parse(User.FindFirstValue("UserId"));
        var result = await Sender.Send(new Command.UserConfirmOrderCommand(userId, order.OrderId, order.IsApproved, order.RejectReason));
        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    [Authorize]
    [HttpPut("lessor_confirm_order", Name = "LessorConfirmOrder")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    public async Task<IActionResult> LessorConfirmOrder([FromBody] OrderRequestConfirmDTO order)
    {
        var userId = Guid.Parse(User.FindFirstValue("UserId"));
        var result = await Sender.Send(new Command.LessorConfirmOrderCommand(userId, order.OrderId, order.IsApproved, order.RejectReason));
        if (result.IsFailure)
            return HandlerFailure(result);
        
        return Ok(result);
    }

    [HttpGet("get_all_orders", Name = "GetAllOrders")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    public async Task<IActionResult> GetAllOrders([FromQuery] OrderFilter filterParams,
    [FromQuery] int pageIndex = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string[] selectedColumns = null)
    {
        var result = await Sender.Send(new Query.GetAllOrdersQuery(pageIndex, pageSize, filterParams, selectedColumns));
        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    [HttpGet("get_details_order", Name = "GetDetailsOrder")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    public async Task<IActionResult> GetDetailsOrder([FromQuery] Guid Id)
    {
        var result = await Sender.Send(new Query.GetOrderByIdQuery(Id));
        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }
}