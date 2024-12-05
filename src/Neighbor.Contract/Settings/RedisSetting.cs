namespace Neighbor.Contract.Settings;

public class RedisSetting
{
    public const string SectionName = "RedisConfiguration";
    public bool Enabled { get; set; }
    public string ConnectionString { get; set; }
}