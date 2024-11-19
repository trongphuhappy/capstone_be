using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Services.Surcharges;
using Neighbor.Presentation.Abstractions;
using static Neighbor.Contract.Services.Surcharges.Filter;

namespace Neighbor.Presentation.Controller.V2;

public class SurchargeController : ApiController
{
    public SurchargeController(ISender sender) : base(sender)
    { }

    [HttpGet("get_all_surcharges", Name = "GetAllSurcharges")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    public async Task<IActionResult> GetAllSurcharges([FromQuery] SurchargeFilter filterParams,
    [FromQuery] int pageIndex = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string[] selectedColumns = null)
    {
        var result = await Sender.Send(new Query.GetAllSurchargesQuery(pageIndex, pageSize, filterParams, selectedColumns));
        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    //[HttpPost("get_surcharge_by_id", Name = "GetSurchargeById")]
    //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    //[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    //public async Task<IActionResult> GetSurchargeById([FromQuery] Guid Id)
    //{
    //    var result = await Sender.Send(new Query.GetAllCategoriesQuery(pageIndex, pageSize, filterParams, selectedColumns));
    //    if (result.IsFailure)
    //        return HandlerFailure(result);

    //    return Ok(result);
    //}
}