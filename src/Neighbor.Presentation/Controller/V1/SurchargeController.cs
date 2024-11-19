using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Services.Authentications;
using Neighbor.Presentation.Abstractions;

namespace Neighbor.Presentation.Controller.V1;

public class SurchargeController : ApiController
{
    public SurchargeController(ISender sender) : base(sender)
    { }

    [HttpPost("get_all_surcharges", Name = "GetAllSurcharges")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    public async Task<IActionResult> GetAllSurcharges([FromBody] Command.RegisterCommand commands)
    {
        var result = await Sender.Send(commands);
        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    [HttpPost("get_surcharge_by_id", Name = "GetSurchargeById")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    public async Task<IActionResult> GetSurchargeById([FromBody] Command.VerifyEmailCommand commands)
    {
        var result = await Sender.Send(commands);
        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }
}