using System.Net.Sockets;
using System.Text;
using ctf_sandbox.tests.Extensions;
using Microsoft.Extensions.Configuration;

namespace ctf_sandbox.tests.SmokeTests;

public class ExternalSystemsHealthTests
{
    [Trait("Category", "Smoke_ExternalSystemsHealth")]
    [Fact]
    public async Task Mailpit_ShouldBeUpAndRunning()
    {
        var configBuilder = new ConfigurationBuilder();
        configBuilder.Sources.Clear();
        configBuilder.AddJsonFile("appsettings.tests.json", optional: false)
        .AddJsonFile("appsettings.tests.dev.json", optional: true)
        .AddEnvironmentVariables();
        var config = configBuilder.Build();
        var url = config.GetRequiredValue<string>("MailPit:Url");
        var smtpServer = config.GetRequiredValue<string>("MailPit:SmtpServer");
        var smtpPort = config.GetRequiredValue<int>("MailPit:SmtpPort");

        // Navigate to Mailpit URL
        HttpResponseMessage response;
        using (var client = new HttpClient())
        {
            response = await client.GetAsync(url);
        }
        // Check if the response is successful
        Assert.True(response.IsSuccessStatusCode, $"Failed to connect to Mailpit at {url}");
        // Check if the SMTP server is reachable
        using (var client = new TcpClient())
        {
            var connectTask = client.ConnectAsync(smtpServer, smtpPort);
            var timeoutTask = Task.Delay(10000); // 10 seconds timeout
            // Wait for either the connection to succeed or the timeout
            var completed = await Task.WhenAny(connectTask, timeoutTask);
            if (completed == timeoutTask)
            {
                Assert.Fail($"Failed to connect to SMTP server {smtpServer}:{smtpPort} within the timeout period.");
            }
            if (!connectTask.IsCompletedSuccessfully)
            {
                Assert.Fail($"Failed to connect to SMTP server {smtpServer}:{smtpPort}. Connection task did not complete successfully.");
            }

            using var stream = client.GetStream();
            var buffer = new byte[1024];
            var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            var banner = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            Assert.Contains("220", banner, StringComparison.OrdinalIgnoreCase);
        }
    }
}
