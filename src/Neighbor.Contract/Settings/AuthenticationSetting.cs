namespace Neighbor.Contract.Settings;

public class AuthenticationSetting
{
    public const string SectionName = "AuthenticationConfiguration";
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string AccessSecretToken { get; set; }
    public string RefreshSecretToken { get; set; }
    public double AccessTokenExpMinute { get; set; }
    public double RefreshTokenExpMinute { get; set; }
}
