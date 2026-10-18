
using ctf_sandbox.Time;

namespace ctf_sandbox.tests.Fixtures;

public class CTFConfiguration
{
    public string WebServerUrl { get; private set; }

    public string MailpitUrl { get; private set; }

    public string IpInfoUrl { get; private set; }

    public string BannedWordsUrl { get; private set; }

    public string DatabaseConnectionString { get; private set; }

    public Credentials WebServerCredentials { get; private set; }

    public Credentials MailpitCredentials { get; private set; }

    public TimeProviderOptions TimeProviderOptions { get; private set; }

    public CTFConfiguration(
        string webServerUrl,
        string mailpitUrl,
        string ipInfoUrl,
        string bannedWordsUrl,
        string databaseConnectionString,
        Credentials webServerCredentials,
        Credentials mailpitCredentials,
        TimeProviderOptions timeProviderOptions)
    {
        WebServerUrl = webServerUrl;
        MailpitUrl = mailpitUrl;
        IpInfoUrl = ipInfoUrl;
        BannedWordsUrl = bannedWordsUrl;
        DatabaseConnectionString = databaseConnectionString;
        WebServerCredentials = webServerCredentials;
        MailpitCredentials = mailpitCredentials;
        TimeProviderOptions = timeProviderOptions;
    }
}
