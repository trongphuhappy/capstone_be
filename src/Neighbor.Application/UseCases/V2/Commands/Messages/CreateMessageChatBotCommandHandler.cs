using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.DTOs.MessageDTOs;
using Neighbor.Contract.Services.Messages;

namespace Neighbor.Application.UseCases.V2.Commands.Messages;

public sealed class CreateMessageChatBotCommandHandler : ICommandHandler<Command.CreateMessageChatBoxCommand, Success<CreateMessageDTO>>
{
    private readonly IDialogflowService _dialogflowService;

    public CreateMessageChatBotCommandHandler(IDialogflowService dialogflowService)
    {
        _dialogflowService = dialogflowService;
    }

    public async Task<Result<Success<CreateMessageDTO>>> Handle(Command.CreateMessageChatBoxCommand request, CancellationToken cancellationToken)
    {
        string languageCode = "vi";
        var textResponse = await _dialogflowService.DetectIntentAsync(request.SessionId, request.Content, languageCode);
        var result = new CreateMessageDTO
        {
            Content = textResponse,
        };

        return Result.Success(new Success<CreateMessageDTO>("", "", result));
    }
}
