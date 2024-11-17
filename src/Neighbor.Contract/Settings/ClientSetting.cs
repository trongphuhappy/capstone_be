namespace Neighbor.Contract.Settings;

public class ClientSetting
{
    public const string SectionName = "ClientConfiguration";
    public string Url { get; set; }
    public string VerifyEmail { get; set; }
    public string VerifyChangeEmail { get; set; }
    public string DonateSuccess { get; set; }
    public string DonateFail { get; set; }
    public string VerifyChangePassword { get; set; }
}

