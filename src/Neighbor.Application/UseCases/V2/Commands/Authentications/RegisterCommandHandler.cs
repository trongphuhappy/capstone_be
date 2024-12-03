using MediatR;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Contract.Services.Authentications;
using Neighbor.Domain.Abstraction.EntitiyFramework;
using Neighbor.Domain.Exceptions;
using System.Text.Json;

namespace Neighbor.Application.UseCases.V2.Commands.Authentications;

public sealed class RegisterCommandHandler : ICommandHandler<Command.RegisterCommand>
{
    private readonly IResponseCacheService _responseCacheService;
    private readonly IEFUnitOfWork _eFUnitOfWork;
    private readonly IPublisher _publisher;

    public RegisterCommandHandler
        (IResponseCacheService responseCacheService,
        IEFUnitOfWork eFUnitOfWork,
        IPublisher publisher)
    {
        _responseCacheService = responseCacheService;
        _eFUnitOfWork = eFUnitOfWork;
        _publisher = publisher;
    }

    public async Task<Result> Handle(Command.RegisterCommand request, CancellationToken cancellationToken)
    {
        // Check email of user have exist. With case user exist => Exception to notification
        var existUser = await _eFUnitOfWork.AccountRepository.AnyAsync(x => x.Email == request.Email);
        // Case user exist
        if (existUser == true)
            throw new AuthenticationException.EmailExistException();
        // Case user not exist
        // Save [register_email]: request, instead of save database to user will verify email
        await _responseCacheService.SetCacheResponseAsync
            ($"register_{request.Email}",
            JsonSerializer.Serialize(request),
            TimeSpan.FromHours(12));
        
        // Send mail to notification user registed, and wait user accept
        await Task.WhenAll(
            _publisher.Publish(new DomainEvent.UserRegistedWithLocal(Guid.NewGuid(), request.Email), cancellationToken)
        );
        
        return Result.Success(new Success(MessagesList.AuthRegisterSuccess.GetMessage().Code, MessagesList.AuthRegisterSuccess.GetMessage().Message));
    }
}
