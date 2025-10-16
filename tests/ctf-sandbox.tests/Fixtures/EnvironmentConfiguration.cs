
namespace ctf_sandbox.tests.Fixtures;

public class EnvironmentConfiguration
{
    public string WebServerUrl { get; private set; }

    public string MailpitUrl { get; private set; }

    public string IpInfoUrl { get; private set; }

    public string DatabaseConnectionString { get; private set; }

    public Credentials WebServerCredentials { get; private set; }

    public Credentials MailpitCredentials { get; private set; }

    public EnvironmentConfiguration(
        string webServerUrl,
        string mailpitUrl,
        string ipInfoUrl,
        string databaseConnectionString,
        Credentials webServerCredentials,
        Credentials mailpitCredentials)
    {
        WebServerUrl = webServerUrl;
        MailpitUrl = mailpitUrl;
        IpInfoUrl = ipInfoUrl;
        DatabaseConnectionString = databaseConnectionString;
        WebServerCredentials = webServerCredentials;
        MailpitCredentials = mailpitCredentials;
    }
}
