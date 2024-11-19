namespace Neighbor.Contract.Services.Authentications;

public static class Response
{
    public record LoginResponse
     (Guid UserId,
     string FirstName,
     string LastName,
     string CropAvatarLink,
     string FullAvatarLink,
     int RoleId,
     string AccessToken,
     string RefreshToken);

    public record RefreshTokenResponse
        (string AccessToken,
        string RefreshToken);
}
