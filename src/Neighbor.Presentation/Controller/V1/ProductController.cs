using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Services.Authentications;
using Neighbor.Presentation.Abstractions;

namespace Neighbor.Presentation.Controller.V1;

public class ProductController : ApiController
{
    public ProductController(ISender sender) : base(sender)
    { }

    //[HttpPost("register", Name = "Register")]
    //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    //[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    //public async Task<IActionResult> Register([FromBody] Command.RegisterCommand commands)
    //{
    //    var result = await Sender.Send(commands);
    //    if (result.IsFailure)
    //        return HandlerFailure(result);

    //    return Ok(result);
    //}

    //[HttpPost("verify-email", Name = "VerifyEmail")]
    //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    //[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    //public async Task<IActionResult> VerifyEmail([FromBody] Command.VerifyEmailCommand commands)
    //{
    //    var result = await Sender.Send(commands);
    //    if (result.IsFailure)
    //        return HandlerFailure(result);

    //    return Ok(result);
    //}

    //[HttpPost("login", Name = "Login")]
    //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    //[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    //public async Task<IActionResult> Login([FromBody] Query.LoginQuery queries)
    //{
    //    var result = await Sender.Send(queries);
    //    if (result.IsFailure)
    //        return HandlerFailure(result);

    //    return Ok(result);
    //}
}