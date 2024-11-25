using MediatR;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Enumarations.Authentication;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Contract.Services.Members;
using Neighbor.Domain.Abstraction.EntitiyFramework;
using static Neighbor.Domain.Exceptions.AccountException;
using static Neighbor.Domain.Exceptions.AuthenticationException;

namespace Neighbor.Application.UseCases.V2.Commands.Members;

public sealed class UpdateEmailCommandHandler : ICommandHandler<Command.UpdateEmailCommand>
{
    private readonly IPublisher _publisher;
    private readonly IResponseCacheService _responseCacheService;
    private readonly IEFUnitOfWork _efUnitOfWork;

    public UpdateEmailCommandHandler(IPublisher publisher,
        IResponseCacheService responseCacheService,
        IEFUnitOfWork efUnitOfWork)
    {
        _publisher = publisher;
        _responseCacheService = responseCacheService;
        _efUnitOfWork = efUnitOfWork;
    }

    public async Task<Result> Handle(Command.UpdateEmailCommand request, CancellationToken cancellationToken)
    {
        var isCheckMail = await _efUnitOfWork.AccountRepository.AnyAsync(account => account.Email == request.Email);
        if (isCheckMail == true)
            throw new AccountUpdateEmailExit();

        var account = await _efUnitOfWork.AccountRepository.FindByIdAsync(request.UserId);
        if (account.IsDeleted == true) throw new AccountBanned();
        if (account.LoginType != LoginType.Local)
            throw new AccountRegisteredAnotherMethodException();

        await _responseCacheService.SetCacheResponseAsync($"changeemail_{request.UserId}", request.Email, TimeSpan.FromMinutes(30));
        await Task.WhenAll(
            _publisher.Publish(new DomainEvent.UserEmailChanged(Guid.NewGuid(), request.UserId, account.Email), cancellationToken)
        );

        return Result.Success(new Success(MessagesList.AccountUpdateChangeEmail.GetMessage().Code, MessagesList.AccountUpdateChangeEmail.GetMessage().Message));
    }
}
