using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.DTOs.MemberDTOs;
using Neighbor.Contract.Enumarations.MessagesList;
using Neighbor.Contract.Services.Members;
using Neighbor.Domain.Abstraction.EntitiyFramework;
using static Neighbor.Domain.Exceptions.AuthenticationException;

namespace Neighbor.Application.UseCases.V2.Commands.Members;

public sealed class UpdateCitizenCommandHandler : ICommandHandler<Command.UpdateCitizenCommand>
{
    private readonly IEFUnitOfWork _efUnitOfWork;
    private readonly IMediaService _mediaService;

    public UpdateCitizenCommandHandler(IEFUnitOfWork efUnitOfWork, IMediaService mediaService)
    {
        _efUnitOfWork = efUnitOfWork;
        _mediaService = mediaService;
    }

    public async Task<Result> Handle(Command.UpdateCitizenCommand request, CancellationToken cancellationToken)
    {
        var user = await _efUnitOfWork.AccountRepository.FindByIdAsync(request.UserId, cancellationToken);
        if (user.IsDeleted == true) throw new AccountBanned();

        var frontImage = await _mediaService.UploadImageAsync($"front_citizen_{user.Id}", request.FrontCitzenImage);
        var backImage = await _mediaService.UploadImageAsync($"back_citizen_{user.Id}", request.BackCitizenImage);

        user.UpdateCitizen(request.CitizenId, frontImage.ImageUrl, frontImage.PublicImageId, backImage.ImageUrl, backImage.PublicImageId);
        _efUnitOfWork.AccountRepository.Update(user);
        await _efUnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new Success(MessagesList.UpdateCitizenSuccessfully.GetMessage().Code, MessagesList.UpdateCitizenSuccessfully.GetMessage().Message));
    }
}
