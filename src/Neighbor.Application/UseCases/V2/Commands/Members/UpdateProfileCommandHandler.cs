using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.DTOs.MemberDTOs;
using Neighbor.Contract.Services.Members;
using Neighbor.Domain.Abstraction.EntitiyFramework;
using static Neighbor.Domain.Exceptions.AuthenticationException;

namespace Neighbor.Application.UseCases.V2.Commands.Members;

public sealed class UpdateProfileCommandHandler : ICommandHandler<Command.UpdateProfileCommand, Success<ProfileDTO>>
{
    private readonly IEFUnitOfWork _efUnitOfWork;

    public UpdateProfileCommandHandler(IEFUnitOfWork efUnitOfWork)
    {
        _efUnitOfWork = efUnitOfWork;
    }

    public async Task<Result<Success<ProfileDTO>>> Handle(Command.UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _efUnitOfWork.AccountRepository.FindByIdAsync(request.UserId, cancellationToken);
        if (user.IsDeleted == true) throw new AccountBanned();

        user.UpdateProfile(request.FirstName, request.LastName, request.Biography, request.PhoneNumber);

        await _efUnitOfWork.SaveChangesAsync(cancellationToken);
        var profileDTO = new ProfileDTO
        {
            Biography = request.Biography,
            PhoneNumber = request.PhoneNumber,
            FirstName = request.FirstName,
            LastName = request.LastName,
        };
        return Result.Success(new Success<ProfileDTO>("", "", profileDTO));
    }
}
