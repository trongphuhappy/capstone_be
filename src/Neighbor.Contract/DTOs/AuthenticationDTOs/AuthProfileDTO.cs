namespace Neighbor.Contract.DTOs.AuthenticationDTOs;

public sealed class AuthProfileDTO
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string CropAvatarLink { get; set; }
    public string FullAvatarLink { get; set; }
    public int RoleId { get; set; }
}
