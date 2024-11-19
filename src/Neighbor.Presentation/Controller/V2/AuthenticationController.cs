using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.DTOs.AuthenticationDTOs;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Contract.Services.Authentications;
using Neighbor.Presentation.Abstractions;
using System.Security.Claims;
using static Neighbor.Domain.Exceptions.AuthenticationException;

namespace Neighbor.Presentation.Controller.V2;

public class AuthenticationController : ApiController
{
    public AuthenticationController(ISender sender) : base(sender)
    { }

    [HttpPost("register", Name = "Register")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    public async Task<IActionResult> Register([FromBody] Command.RegisterCommand commands)
    {
        var result = await Sender.Send(commands);
        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    [HttpPost("verify-email", Name = "VerifyEmail")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    public async Task<IActionResult> VerifyEmail([FromBody] Command.VerifyEmailCommand commands)
    {
        var result = await Sender.Send(commands);
        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    [HttpPost("login", Name = "Login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    public async Task<IActionResult> Login([FromBody] Query.LoginQuery Login)
    {
        var result = await Sender.Send(Login);
        if (result.IsFailure)
            return HandlerFailure(result);

        var value = result.Value;

        Response.Cookies.Append("refreshToken", value.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Path = "/",
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.Now.AddMinutes(131400),
        });

        var authProfileDTO = new AuthProfileDTO()
        {
            UserId = value.UserId,
            FirstName = value.FirstName,
            LastName = value.LastName,
            CropAvatarLink = value.CropAvatarLink,
            FullAvatarLink = value.FullAvatarLink,
            RoleId = value.RoleId,
        };

        var tokenDto = new TokenDTO()
        {
            AccessToken = value.AccessToken,
            TokenType = "Bearer"
        };

        return Ok(new
        {
            AuthProfile = authProfileDTO,
            Token = tokenDto,
        });
    }

    [HttpGet("refresh-token", Name = "RefreshToken")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (refreshToken == null) throw new RefreshTokenNullException();

        var result = await Sender.Send(new Query.RefreshTokenQuery(refreshToken));
        if (result.IsFailure)
            return HandlerFailure(result);

        var value = result.Value;

        Response.Cookies.Append("refreshToken", value.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Path = "/",
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.Now.AddMinutes(131400),
        });

        return Ok(new
        {
            TokenType = "Bearer",
            value.AccessToken,
        });
    }

    [Authorize]
    [HttpPost("logout", Name = "Logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Logout()
    {
        var user = User.FindFirstValue("UserId");
        Response.Cookies.Delete("refreshToken");
        return Ok(Result.Success(new Success(MessagesList.AuthLogoutSuccess.GetMessage().Code,
            MessagesList.AuthLogoutSuccess.GetMessage().Message)));
    }
}