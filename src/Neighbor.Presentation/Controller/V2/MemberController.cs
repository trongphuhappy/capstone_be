using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Services.Members;
using Neighbor.Presentation.Abstractions;
using System.Security.Claims;

namespace Neighbor.Presentation.Controller.V2;

[ApiVersion(2)]
public class MemberController : ApiController
{
    public MemberController(ISender sender) : base(sender)
    { }

    [Authorize]
    [HttpGet("get_profile", Name = "GetProfile")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirstValue("UserId");
        var result = await Sender.Send(new Query.GetProfileQuery(Guid.Parse(userId)));
        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }
}
