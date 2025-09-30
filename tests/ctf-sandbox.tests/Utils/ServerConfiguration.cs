using Microsoft.Extensions.Configuration;

namespace ctf_sandbox.tests.Fixtures.Utils;

public class ServerConfiguration
{
    public string? WebServerUrl { get; set; }

    public string? MailpitUrl { get; private set; }

    public string? IpInfoUrl { get; private set; }

    public string? DatabaseConnectionString { get; private set; }

    public Credentials WebServerCredentials { get; private set; }

    public Credentials MailpitCredentials { get; private set; }

    public ServerConfiguration()
    {
        var configBuilder = new ConfigurationBuilder();
        configBuilder.Sources.Clear();
        configBuilder.AddJsonFile("appsettings.tests.json", optional: false)
        .AddJsonFile("appsettings.tests.dev.json", optional: true)
        .AddEnvironmentVariables();
        var config = configBuilder.Build();
        WebServerUrl = config.GetValue<string>("WebServer:Url");
        MailpitUrl = config.GetValue<string>("Mailpit:Url");
        IpInfoUrl = config.GetValue<string>("IpInfo:Url");
        DatabaseConnectionString = config.GetValue<string>("Database:ConnectionString");

        WebServerCredentials = new Credentials(
            config.GetValue<string>("WebServer:AdminAccount")!,
            config.GetValue<string>("WebServer:AdminPassword")!
        );

        MailpitCredentials = new Credentials(
            config.GetValue<string>("Mailpit:AdminAccount")!,
            config.GetValue<string>("Mailpit:AdminPassword")!
        );
    }
}
