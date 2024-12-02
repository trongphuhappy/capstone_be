namespace Neighbor.Contract.DTOs.OrderDTOs;

public sealed class UserDTO
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string CropAvatarLink { get; set; }
    public string FullAvatarLink { get; set; }
}
