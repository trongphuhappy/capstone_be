namespace Neighbor.Contract.Settings;

public class ClientSetting
{
    public const string SectionName = "ClientConfiguration";
    public string Url { get; set; }
    public string VerifyEmail { get; set; }
    public string VerifyChangeEmail { get; set; }
    public string OrderSuccess { get; set; }
    public string OrderFail { get; set; }
    public string VerifyChangePassword { get; set; }
}

