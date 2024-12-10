using Asp.Versioning;
using MediatR;
using Neighbor.Presentation.Abstractions;

namespace Neighbor.Presentation.Controller.V2;

[ApiVersion(2)]
public class StatisticController : ApiController
{
    public StatisticController(ISender sender) : base(sender)
    { }

    //[Authorize]
    //[HttpGet("total_statistic_lessor", Name = "TotalStatisticLessor")]
    //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    //[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    //public async Task<IActionResult> TotalStatisticLessor([FromQuery] Command.HandleUserCommand commands)
    //{
    //    var result = await Sender.Send(commands);
    //    if (result.IsFailure)
    //        return HandlerFailure(result);

    //    return Ok(result);
    //}
}