using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using ctf_sandbox.tests.Fixtures;

namespace ctf_sandbox.tests.SmokeTests;

[Collection(RealExternalSystemsTestCollection.Name)]
public class ExternalSystemsHealthTests : EnvironmentTests
{
    public ExternalSystemsHealthTests(RealExternalSystemsEnvironmentFixture fixture) : base(fixture)
    {
    }

    [Trait("Category", "Smoke_ExternalSystemsHealth")]
    [Fact]
    public async Task Mailpit_ShouldBeUpAndRunning()
    {
        // Navigate to Mailpit URL
        HttpResponseMessage response;
        using (var client = new HttpClient())
        {
            if (!EnvironmentFixture.Configuration.MailpitCredentials.IsEmpty())
            {
                var byteArray = Encoding.ASCII.GetBytes($"{EnvironmentFixture.Configuration.MailpitCredentials.Username}:{EnvironmentFixture.Configuration.MailpitCredentials.Password}");
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            }
            response = await client.GetAsync(EnvironmentFixture.Configuration.MailpitUrl);
        }
        // Check if the response is successful
        Assert.True(response.IsSuccessStatusCode, $"Failed to connect to Mailpit at {EnvironmentFixture.Configuration.MailpitUrl}, status code: {response.StatusCode}");
    }

    [Fact]
    [Trait("Category", "Smoke_ExternalSystemsHealth")]
    public async Task IpInfo_ShouldBeUpAndRunning()
    {
        // Ensure we have a URL configured
        Assert.NotNull(EnvironmentFixture.Configuration.IpInfoUrl);

        // Parse the URL to get host and port
        var uri = new Uri(EnvironmentFixture.Configuration.IpInfoUrl);
        var port = uri.Port == -1 ? (uri.Scheme == "https" ? 443 : 80) : uri.Port;

        using var client = new TcpClient();
        var connectTask = client.ConnectAsync(uri.Host, port);
        // Use a reasonable timeout
        var timeoutTask = Task.Delay(TimeSpan.FromSeconds(10));

        // Wait for either connection or timeout
        var completedTask = await Task.WhenAny(connectTask, timeoutTask);

        Assert.True(completedTask == connectTask,
            $"Failed to establish TCP connection to {uri.Host}:{port} within timeout period");
        Assert.True(client.Connected,
            $"TCP connection to {uri.Host}:{port} was not established successfully");
    }
}
