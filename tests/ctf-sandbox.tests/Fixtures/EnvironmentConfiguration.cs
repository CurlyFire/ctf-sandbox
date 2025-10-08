
namespace ctf_sandbox.tests.Fixtures;

public class EnvironmentConfiguration
{
    public string? WebServerUrl { get; set; }

    public string? MailpitUrl { get; private set; }

    public string? IpInfoUrl { get; private set; }

    public string? DatabaseConnectionString { get; private set; }

    public Credentials? WebServerCredentials { get; private set; }

    public Credentials? MailpitCredentials { get; private set; }
}
