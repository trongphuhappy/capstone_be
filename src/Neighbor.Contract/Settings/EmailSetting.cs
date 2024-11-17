namespace Neighbor.Contract.Settings;

public class EmailSetting
{
    public const string SectionName = "EmailConfiguration";
    public string EmailHost { get; set; }
    public string EmailUsername { get; set; }
    public string EmailPassword { get; set; }
}
