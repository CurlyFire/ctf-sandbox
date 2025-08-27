using Microsoft.Extensions.Configuration;

namespace ctf_sandbox.tests;

public class ServerConfiguration
{
    public string? WebServerUrl { get; protected set; }

    public string? MailPitUrl { get; private set; }

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
        MailPitUrl = config.GetValue<string>("MailPit:Url");

        WebServerCredentials = new Credentials(
            config.GetValue<string>("WebServer:AdminAccount")!,
            config.GetValue<string>("WebServer:AdminPassword")!
        );

        MailpitCredentials = new Credentials(
            config.GetValue<string>("MailPit:AdminAccount")!,
            config.GetValue<string>("MailPit:AdminPassword")!
        );
    }
}
