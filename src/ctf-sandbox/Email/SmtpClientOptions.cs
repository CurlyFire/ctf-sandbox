namespace ctf_sandbox.Email;

public class SmtpClientOptions
{
    public const string ConfigurationSection = "EmailSettings";
    public string Type { get; set; } = "smtp";
    public string? SmtpServer { get; set; }
    public int SmtpPort { get; set; } = 1025;
    public string? Url { get; set; }
}
