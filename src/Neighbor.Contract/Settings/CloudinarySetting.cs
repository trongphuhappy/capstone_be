namespace Neighbor.Contract.Settings;

public class CloudinarySetting
{
    public const string SectionName = "CloudinarySetting";
    public string CloudName { get; set; }
    public string ApiKey { get; set; }
    public string ApiSecret { get; set; }
    public string Folder { get; set; }
}
