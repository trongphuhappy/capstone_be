using Neighbor.Contract.Enumarations.Authentication;
using Neighbor.Domain.Abstraction.Entities;

namespace Neighbor.Domain.Entities;
public class Account : DomainEntity<Guid>
{
    public Account()
    {
    }

    public Account(string firstName,
        string lastName,
        string email,
        string phoneNumber,
        string password,
        string? cropAvatarUrl,
        string? cropAvatarId,
        string? fullAvatarUrl,
        string? fullAvatarId,
        string? cropCoverPhotoUrl,
        string? cropCoverPhotoId,
        string? fullCoverPhotoUrl,
        string? fullCoverPhotoId,
        string? biography,
        LoginType loginType,
        GenderType genderType,
        RoleType roleUserId,
        bool isDeleted)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        Password = password;
        CropAvatarUrl = cropAvatarUrl;
        CropAvatarId = cropAvatarId;
        FullAvatarUrl = fullAvatarUrl;
        FullAvatarId = fullAvatarId;
        CropCoverPhotoUrl = cropAvatarUrl;
        CropCoverPhotoId = cropAvatarId;
        FullCoverPhotoUrl = fullAvatarUrl;
        FullCoverPhotoId = fullAvatarId;
        Biography = biography;
        LoginType = loginType;
        GenderType = genderType;
        RoleUserId = roleUserId;
        IsDeleted = isDeleted;
    }

    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public string Password { get; private set; } = string.Empty;
    public string? CropAvatarUrl { get; private set; }
    public string? CropAvatarId { get; private set; }
    public string? FullAvatarUrl { get; private set; }
    public string? FullAvatarId { get; private set; }
    public string? CropCoverPhotoUrl { get; private set; }
    public string? CropCoverPhotoId { get; private set; }
    public string? FullCoverPhotoUrl { get; private set; }
    public string? FullCoverPhotoId { get; private set; }
    public string? Biography { get; private set; }
    public LoginType LoginType { get; private set; }
    public GenderType GenderType { get; private set; }
    public RoleType RoleUserId { get; private set; }
    public virtual RoleUser RoleUser { get; private set; }
    public virtual List<Lessor>? Lessor { get;private set; }
    public virtual List<Wishlist>? Wishlists { get; private set; }
    public virtual List<Order>? Orders { get; private set; }
    
    public static Account CreateMemberAccountLocal(string firstName, string lastName, string email, string phoneNumber, string password, string avatarUrl, GenderType gender)
    {
        return new Account(firstName, lastName, email, phoneNumber, password, avatarUrl, "", avatarUrl, "", "", "", "", "", "", LoginType.Local, gender, RoleType.Member, false);
    }
    public static Account CreateAccountAdminLocal(string email, string password, string avatarUrl)
    {
        return new Account("Admin", "", email, "", password, avatarUrl, "", avatarUrl, "", "", "", "", "", "", LoginType.Local, GenderType.Male, RoleType.Admin, false);
    }

    public void UpdateIsDeletedForAccount(bool isDeleted)
    {
        IsDeleted = isDeleted;
    }

    public void UpdateAvatarUser(string cropAvatarUrl, string cropAvatarId, string fullAvatarUrl, string fullAvatarId)
    {
        CropAvatarId = cropAvatarId;
        CropAvatarUrl = cropAvatarUrl;
        FullAvatarId = fullAvatarId;
        FullAvatarUrl = fullAvatarUrl;
    }

    public void UpdateCoverPhoto(string cropCoverPhotoUrl, string cropCoverPhotoId, string fullCoverPhotoUrl, string fullCoverPhotoId)
    {
        CropCoverPhotoId = cropCoverPhotoId;
        CropCoverPhotoUrl = cropCoverPhotoUrl;
        FullCoverPhotoId = fullCoverPhotoId;
        FullCoverPhotoUrl = fullCoverPhotoUrl;
    }

    public void UpdateProfile(string? firstName, string? lastName, string? biography, string? phoneNumber, string? email)
    {
        if (firstName != null) FirstName = firstName;
        if (lastName != null) LastName = lastName;
        if (biography != null) Biography = biography;
        if (phoneNumber != null) PhoneNumber = phoneNumber;
        if (email != null) Email = email;
    }
}
