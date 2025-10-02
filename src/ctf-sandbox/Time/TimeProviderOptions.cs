namespace ctf_sandbox.Time;

public class TimeProviderOptions
{
    public const string ConfigurationSection = "TimeProvider";
    public string Type { get; set; } = "system";
    public string? Url { get; set; }
}
