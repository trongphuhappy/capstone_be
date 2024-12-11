using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Services.Statistic;
using Neighbor.Presentation.Abstractions;
using System.Security.Claims;

namespace Neighbor.Presentation.Controller.V2;

[ApiVersion(2)]
public class StatisticController : ApiController
{
    public StatisticController(ISender sender) : base(sender)
    { }

    [Authorize]
    [HttpGet("get_total_box_lessor", Name = "GetTotalBoxLessorAsync")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    public async Task<IActionResult> GetTotalBoxLessor()
    {
        var userId = Guid.Parse(User.FindFirstValue("UserId"));
        var result = await Sender.Send(new Query.GetTotalBoxLessorQuery(userId));
        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    [Authorize]
    [HttpGet("get_count_order_by_lessor", Name = "GetCountOrderByLessorQuery")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    public async Task<IActionResult> GetCountOrderByLessor()
    {
        var userId = Guid.Parse(User.FindFirstValue("UserId"));
        var result = await Sender.Send(new Query.GetCountOrderByLessorQuery(userId));
        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    [HttpGet("get_percent_by_lessor", Name = "GetPercenByLessor")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    public async Task<IActionResult> GetPercentByLessor()
    {
        var userId = Guid.Parse(User.FindFirstValue("UserId"));
        var result = await Sender.Send(new Query.GetPercentByLessorQuery(userId));
        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

}
