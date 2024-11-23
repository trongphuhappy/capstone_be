using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.DTOs.MediaDTOs;
using Neighbor.Contract.DTOs.MemberDTOs;
using Neighbor.Contract.Services.Members;
using Neighbor.Domain.Abstraction.EntitiyFramework;
using static Neighbor.Domain.Exceptions.AuthenticationException;

namespace Neighbor.Application.UseCases.V2.Commands.Members;

public sealed class UpdateCoverPhotoCommandHandler : ICommandHandler<Command.UpdateCoverPhotoCommand, Success<ImageProfileDTO>>
{
    private readonly IEFUnitOfWork _efUnitOfWork;
    private readonly IMediaService _mediaService;

    public UpdateCoverPhotoCommandHandler(IEFUnitOfWork efUnitOfWork, IMediaService mediaService)
    {
        _efUnitOfWork = efUnitOfWork;
        _mediaService = mediaService;
    }

    public async Task<Result<Success<ImageProfileDTO>>> Handle(Command.UpdateCoverPhotoCommand request, CancellationToken cancellationToken)
    {
        var user = await _efUnitOfWork.AccountRepository.FindByIdAsync(request.UserId, cancellationToken);
        if(user.IsDeleted == true) throw new AccountBanned();

        var cropCoverPhoto = await _mediaService.UploadImageAsync($"crop_avatar_{user.Id}", request.CropCoverPhoto);
        var fullCoverPhoto = await _mediaService.UploadImageAsync($"full_avatar_{user.Id}", request.FullCoverPhoto);

        user.UpdateCoverPhoto(cropCoverPhoto.ImageUrl, cropCoverPhoto.PublicImageId, fullCoverPhoto.ImageUrl, fullCoverPhoto.PublicImageId);
        _efUnitOfWork.AccountRepository.Update(user);
        await _efUnitOfWork.SaveChangesAsync(cancellationToken);
        
        var imageProfileDTO = new ImageProfileDTO
        {
            CropCoverPhotoUrl = cropCoverPhoto.ImageUrl,
            CropCoverPhotoId = cropCoverPhoto.PublicImageId,
            FullCoverPhotoUrl = fullCoverPhoto.ImageUrl,
            FullCoverPhotoId = fullCoverPhoto.PublicImageId,
        };
        
        return Result.Success(new Success<ImageProfileDTO>("", "", imageProfileDTO));
    }
}
