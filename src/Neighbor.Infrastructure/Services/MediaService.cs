using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Settings;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Neighbor.Contract.DTOs.MediaDTOs;

namespace Neighbor.Infrastructure.Services;

public class MediaService : IMediaService
{
    private readonly CloudinarySetting _cloudinarySetting;
    private readonly Cloudinary _cloudinary;
    public MediaService(IOptions<CloudinarySetting> cloudinaryConfig)
    {
        var account = new Account(cloudinaryConfig.Value.CloudName,
            cloudinaryConfig.Value.ApiKey,
            cloudinaryConfig.Value.ApiSecret);

        _cloudinary = new Cloudinary(account);
        _cloudinarySetting = cloudinaryConfig.Value;
    }

    public async Task<bool> DeleteFileAsync(string publicId)
    {
        var deletionParams = new DeletionParams(publicId);
        var isCheck = await _cloudinary.DestroyAsync(deletionParams);
        if (isCheck.Error != null) return false;
        return true;
    }

    public async Task<ImageDTO> UploadImageAsync(string fileName, IFormFile fileImage)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, fileImage.OpenReadStream()),
            Folder = _cloudinarySetting.Folder,
        };
        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        if (uploadResult?.StatusCode != System.Net.HttpStatusCode.OK) return null;
        var imageUrl = uploadResult.Url.AbsoluteUri;
        var imageId = uploadResult.PublicId;
        return new ImageDTO
        {
            ImageUrl = imageUrl,
            PublicImageId = imageId
        };
    }

    public async Task<List<ImageDTO>> UploadImagesAsync(List<IFormFile> fileImages)
    {
        var imageDtoList = new List<ImageDTO>();

        foreach (var fileImage in fileImages)
        {
            var fileName = fileImage.FileName;

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, fileImage.OpenReadStream()),
                Folder = _cloudinarySetting.Folder,
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult?.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var imageUrl = uploadResult.Url.AbsoluteUri;
                var imageId = uploadResult.PublicId;

                imageDtoList.Add(new ImageDTO
                {
                    ImageUrl = imageUrl,
                    PublicImageId = imageId
                });
            }
        }

        return imageDtoList;
    }
}
