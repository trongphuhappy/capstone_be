using Microsoft.AspNetCore.Http;
using Neighbor.Contract.Abstractions.Message;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.DTOs.MemberDTOs;

namespace Neighbor.Contract.Services.Members;

public static class Command
{
    public record UpdateAvatarCommand(Guid UserId, IFormFile CropAvatar, IFormFile FullAvatar) : ICommand<Success<ImageProfileDTO>>;
    public record UpdateCoverPhotoCommand(Guid UserId, IFormFile CropCoverPhoto, IFormFile FullCoverPhoto) : ICommand<Success<ImageProfileDTO>>;
    public record UpdateProfileCommand(Guid UserId, string? FirstName, string? LastName, string? Biography, string? PhoneNumber) : ICommand<Success<ProfileDTO>>;
    public record UpdateEmailCommand(Guid UserId, string Email) : ICommand;
    public record VerifyUpdateEmailCommand(Guid UserId) : ICommand;
}
