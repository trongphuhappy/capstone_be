using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.DTOs.MemberDTOs;
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

    [Authorize]
    [HttpPut("update_avatar", Name = "UpdateAvatar")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    public async Task<IActionResult> UpdateAvatar([FromForm] RequestProfileDTO.RequestUpdateAvatar request)
    {
        var userId = User.FindFirstValue("UserId");
        var result = await Sender.Send(new Command.UpdateAvatarCommand(Guid.Parse(userId), request.CropAvatar, request.FullAvatar));
        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    [Authorize]
    [HttpPut("update_cover_photo", Name = "UpdateCoverPhoto")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    public async Task<IActionResult> UpdateCoverPhoto([FromForm] RequestProfileDTO.RequestUpdateCoverPhoto request)
    {
        var userId = User.FindFirstValue("UserId");
        var result = await Sender.Send(new Command.UpdateCoverPhotoCommand(Guid.Parse(userId), request.CropCoverPhoto, request.FullCoverPhoto));
        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    [Authorize]
    [HttpPut("update_profile", Name = "UpdateProfile")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    public async Task<IActionResult> UpdateProfile([FromBody] RequestProfileDTO.RequestUpdateProfile request)
    {
        var userId = User.FindFirstValue("UserId");
        var result = await Sender.Send(new Command.UpdateProfileCommand(Guid.Parse(userId), request.FirstName, request.LastName, request.Biography, request.PhoneNumber));
        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }


    [Authorize]
    [HttpPut("update_email", Name = "UpdateEmail")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    public async Task<IActionResult> UpdateEmail([FromBody] RequestProfileDTO.RequestUdpateEmail request)
    {
        var userId = User.FindFirstValue("UserId");
        var result = await Sender.Send(new Command.UpdateEmailCommand(Guid.Parse(userId), request.Email));
        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    [HttpPut("verify-update-email", Name = "VerifyUpdateEmail")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> VerifyUpdateEmail([FromQuery] string userId)
    {
        var result = await Sender.Send(new Command.VerifyUpdateEmailCommand(Guid.Parse(userId)));
        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }
}
