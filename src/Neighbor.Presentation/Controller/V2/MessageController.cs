using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Services.Messages;
using Neighbor.Presentation.Abstractions;

namespace Neighbor.Presentation.Controller.V2;

[ApiVersion(2)]
public class MessageController : ApiController
{
    public MessageController(ISender sender) : base(sender)
    { }

    [HttpPost("create_message_chatbot", Name = "CreateMessageChatBot")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    public async Task<IActionResult> CreateMessageChatBot([FromBody] Command.CreateMessageChatBoxCommand commands)
    {
        var result = await Sender.Send(commands);
        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }
}
