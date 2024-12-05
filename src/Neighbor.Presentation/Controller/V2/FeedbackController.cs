using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Services.Feedbacks;
using Neighbor.Presentation.Abstractions;
using static Neighbor.Contract.Services.Feedbacks.Filter;

namespace Neighbor.Presentation.Controller.V2;

[ApiVersion(2)]
public class FeedbackController : ApiController
{
    public FeedbackController(ISender sender) : base(sender)
    { }

    [HttpPost("create_feedback", Name = "CreateFeedbackCommand")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    public async Task<IActionResult> CreateFeedback([FromBody] Command.CreateFeedbackCommand commands)
    {
        var result = await Sender.Send(commands);
        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    [HttpGet("get_feedbacks", Name = "GetFeedbacks")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    public async Task<IActionResult> GetFeedbacks([FromQuery] FeedbackFilter filterParams,
    [FromQuery] int pageIndex = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string[] selectedColumns = null)
    {
        var result = await Sender.Send(new Query.GetFeedbacksQuery(pageIndex, pageSize, filterParams, selectedColumns));
        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }
}
