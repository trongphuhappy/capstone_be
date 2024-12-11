using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Services.Admins;
using Neighbor.Presentation.Abstractions;

namespace Neighbor.Presentation.Controller.V2;

[ApiVersion(2)]
public class AdminController : ApiController
{
    public AdminController(ISender sender) : base(sender)
    { }

    [HttpPut("handle_user", Name = "HandleUser")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    public async Task<IActionResult> HandleUser([FromBody] Command.HandleUserCommand commands)
    {
        var result = await Sender.Send(commands);
        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    [HttpGet("get_dashboard", Name = "GetDashboard")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDashboard([FromQuery] Query.GetDashboardQuery getDashboardQuery)
    {
        var result = await Sender.Send(getDashboardQuery);
        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }
}