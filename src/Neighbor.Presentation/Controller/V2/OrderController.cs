using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.DTOs.ProductDTOs;
using Neighbor.Contract.Services.Orders;
using Neighbor.Presentation.Abstractions;

namespace Neighbor.Presentation.Controller.V2;

[ApiVersion(2)]
public class OrderController : ApiController
{
    public OrderController(ISender sender) : base(sender)
    { }

    //[Authorize]
    [HttpPost("create_order", Name = "CreateOrder")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    public async Task<IActionResult> HandleUser([FromBody] OrderDTO.OrderRequestDTO order)
    {
        //var userId = Guid.Parse(User.FindFirstValue("UserId"));
        var userId = Guid.Parse("5F7659FA-43C8-4A0B-B993-D00FD9D91C43");
        var result = await Sender.Send(new Command.CreateOrderBankingCommand(userId, order.ProductId, order.RentTime, order.ReturnTime));
        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    [HttpPut("user_confirm_order", Name = "UserConfirmOrder")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    public async Task<IActionResult> HandleUser([FromBody] Command.UserConfirmOrderCommand commands)
    {
        var result = await Sender.Send(commands);
        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    [HttpPut("lessor_confirm_order", Name = "LessorConfirmOrder")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    public async Task<IActionResult> HandleUser([FromBody] Command.LessorConfirmOrderCommand commands)
    {
        var result = await Sender.Send(commands);
        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    //[HttpGet("get_all_orders", Name = "GetAllOrders")]
    //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    //[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    //public async Task<IActionResult> HandleUser([FromBody] Query.GetAllOrders commands)
    //{
    //    var result = await Sender.Send(commands);
    //    if (result.IsFailure)
    //        return HandlerFailure(result);

    //    return Ok(result);
    //}
}