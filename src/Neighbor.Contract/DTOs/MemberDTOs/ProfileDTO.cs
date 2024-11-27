using Neighbor.Contract.Enumarations.Authentication;

namespace Neighbor.Contract.DTOs.MemberDTOs;
public sealed class ProfileDTO
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Biography { get; set; }
    public string? CitizenId { get; set; }
    public string? CropAvatarUrl { get; set; }
    public string? CropAvatarId { get; set; }
    public string? FullAvatarUrl { get; set; }
    public string? FullAvatarId { get; set; }
    public string? CropCoverPhotoUrl { get; set; }
    public string? CropCoverPhotoId { get; set; }
    public string? FullCoverPhotoUrl { get; set; }
    public string? FullCoverPhotoId { get; set; }
    public LoginType? LoginType { get; set; }
    public GenderType? GenderType { get; set; }
    public RoleType? RoleUserId { get; set; }
    public DateTime? CreatedDate { get; set; }
}
