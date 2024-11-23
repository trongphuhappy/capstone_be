using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.DTOs.MediaDTOs;
using Neighbor.Contract.DTOs.MemberDTOs;
using Neighbor.Contract.Services.Members;
using Neighbor.Domain.Abstraction.EntitiyFramework;
using static Neighbor.Domain.Exceptions.AuthenticationException;

namespace Neighbor.Application.UseCases.V2.Commands.Members;

public sealed class UpdateAvatarCommandHandler : ICommandHandler<Command.UpdateAvatarCommand, Success<ImageProfileDTO>>
{
    private readonly IEFUnitOfWork _efUnitOfWork;
    private readonly IMediaService _mediaService;

    public UpdateAvatarCommandHandler(IEFUnitOfWork efUnitOfWork, IMediaService mediaService)
    {
        _efUnitOfWork = efUnitOfWork;
        _mediaService = mediaService;
    }

    public async Task<Result<Success<ImageProfileDTO>>> Handle(Command.UpdateAvatarCommand request, CancellationToken cancellationToken)
    {
        var user = await _efUnitOfWork.AccountRepository.FindByIdAsync(request.UserId, cancellationToken);
        if(user.IsDeleted == true) throw new AccountBanned();

        var cropAvatar = await _mediaService.UploadImageAsync($"crop_avatar_{user.Id}", request.CropAvatar);
        var fullAvatar = await _mediaService.UploadImageAsync($"full_avatar_{user.Id}", request.FullAvatar);

        user.UpdateAvatarUser(cropAvatar.ImageUrl, cropAvatar.PublicImageId,fullAvatar.ImageUrl, fullAvatar.PublicImageId);
        _efUnitOfWork.AccountRepository.Update(user);
        await _efUnitOfWork.SaveChangesAsync(cancellationToken);
        
        var imageProfileDTO = new ImageProfileDTO
        {
            CropAvatarUrl = cropAvatar.ImageUrl,
            CropAvatarId = cropAvatar.PublicImageId,
            FullAvatarUrl = fullAvatar.ImageUrl,
            FullAvatarId = fullAvatar.PublicImageId,
        };
        
        return Result.Success(new Success<ImageProfileDTO>("", "", imageProfileDTO));

    }
}
