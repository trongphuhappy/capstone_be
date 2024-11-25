using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Domain.Abstraction.EntitiyFramework.Repositories;
using Neighbor.Domain.Abstraction.EntitiyFramework;
using Newtonsoft.Json;
using static Neighbor.Domain.Exceptions.AccountException;
using Neighbor.Contract.Services.Members;
using Neighbor.Domain.Entities;
using static Neighbor.Domain.Exceptions.AuthenticationException;

namespace Neighbor.Application.UseCases.V2.Commands.Members;

public sealed class VerifyUpdateEmailCommandHandler : ICommandHandler<Command.VerifyUpdateEmailCommand>
{
    private readonly IResponseCacheService _responseCacheService;
    private readonly IRepositoryBase<Domain.Entities.Account, Guid> _accountRepository;
    private readonly IEFUnitOfWork _efUnitOfWork;

    public VerifyUpdateEmailCommandHandler
        (IResponseCacheService responseCacheService,
        IRepositoryBase<Domain.Entities.Account, Guid> accountRepository,
        IEFUnitOfWork efUnitOfWork)
    {
        _responseCacheService = responseCacheService;
        _accountRepository = accountRepository;
        _efUnitOfWork = efUnitOfWork;
    }

    public async Task<Result> Handle(Command.VerifyUpdateEmailCommand request, CancellationToken cancellationToken)
    {
        var changeEmailMemory = await _responseCacheService.GetCacheResponseAsync($"changeemail_{request.UserId}");
        var newEmail = JsonConvert.DeserializeObject<string>(changeEmailMemory);
        var account = await _accountRepository.FindByIdAsync(request.UserId);
        if (account == null) throw new AccountNotFoundException();
        if (account.IsDeleted == true) throw new AccountBanned();

        account.UpdateProfile(null, null, null, null, newEmail);
        await _efUnitOfWork.SaveChangesAsync(cancellationToken);

        await _responseCacheService.DeleteCacheResponseAsync($"changeemail_{request.UserId}");

        return Result.Success(new Success(MessagesList.AccountUpdateEmailSuccess.GetMessage().Code,
            MessagesList.AccountUpdateEmailSuccess.GetMessage().Message
            ));
    }
}

