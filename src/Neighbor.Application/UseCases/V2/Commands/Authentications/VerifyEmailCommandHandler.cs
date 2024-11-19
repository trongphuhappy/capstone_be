using MediatR;
using Microsoft.Extensions.Options;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Enumarations.Authentication;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Contract.Services.Authentications;
using Neighbor.Contract.Settings;
using Neighbor.Domain.Abstraction.EntitiyFramework;
using Neighbor.Domain.Abstraction.EntitiyFramework.Repositories;
using Newtonsoft.Json;
using static Neighbor.Domain.Exceptions.AuthenticationException;

namespace Neighbor.Application.UseCases.V2.Commands.Authentications;

public sealed class VerifyEmailCommandHandler : ICommandHandler<Command.VerifyEmailCommand>
{
    private readonly IResponseCacheService _responseCacheService;
    private readonly IEFUnitOfWork _efUnitOfWork;
    private readonly IPasswordHashService _passwordHashService;
    private readonly IPublisher _publisher;
    private readonly UserSetting _userSetting;

    public VerifyEmailCommandHandler
       (IRepositoryBase<Domain.Entities.Account, Guid> accountRepository,
       IResponseCacheService responseCacheService,
       IEFUnitOfWork unitOfWork,
       IPasswordHashService passwordHashService,
       IPublisher publisher,
       IOptions<UserSetting> userConfiguration)
    {
        _responseCacheService = responseCacheService;
        _efUnitOfWork = unitOfWork;
        _passwordHashService = passwordHashService;
        _publisher = publisher;
        _userSetting = userConfiguration.Value;
    }

    public async Task<Result> Handle(Command.VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        // Check email of user have exist. With case user exist => Exception to notification
        var existUser = await _efUnitOfWork.AccountRepository.AnyAsync(x => x.Email == request.Email);

        // Check user have exit in system by email
        if (existUser == true) throw new EmailExistException();
        // Case user not exist
        var registerMemory = await _responseCacheService.GetCacheResponseAsync($"register_{request.Email}");
        // If get with value = null => exception
        if (registerMemory == null) throw new RegisterFailureException();

        string unescapedJson = JsonConvert.DeserializeObject<string>(registerMemory);
        var user = JsonConvert.DeserializeObject<Command.RegisterCommand>(unescapedJson);

        // Hash password
        var passwordHash = _passwordHashService.HashPassword(user.Password);

        // Get avatar if avatar is male or female
        var avatar = user.Gender == GenderType.Male ? _userSetting.DefaultMaleAvatar : _userSetting.DefaultFemaleAvatar;
        // Create object account with type register local
        var accountMember = Domain.Entities.Account.CreateMemberAccountLocal
            (user.FirstName, user.LastName, user.Email, user.PhoneNumber, passwordHash, avatar, user.Gender);

        // Save database
        _efUnitOfWork.AccountRepository.Add(accountMember);
        await _efUnitOfWork.SaveChangesAsync(cancellationToken);

        // Delete object saved in memory
        await _responseCacheService.DeleteCacheResponseAsync($"register_{request.Email}");

        // Send email when verified successfully
        await Task.WhenAll(
                _publisher.Publish(new DomainEvent.UserVerifiedEmailRegist(Guid.NewGuid(), request.Email),
                cancellationToken)
        );

        return Result.Success(new Success(MessagesList.AuthVerifyEmailRegister.GetMessage().Code,
            MessagesList.AuthVerifyEmailRegister.GetMessage().Message));
    }
}
