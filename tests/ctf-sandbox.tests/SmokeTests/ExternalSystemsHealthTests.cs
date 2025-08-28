using System.Net.Http.Headers;
using System.Text;

namespace ctf_sandbox.tests.SmokeTests;

public class ExternalSystemsHealthTests : IClassFixture<ServerConfiguration>
{
    private readonly ServerConfiguration _serverConfiguration;

    public ExternalSystemsHealthTests(ServerConfiguration serverConfiguration)
    {
        _serverConfiguration = serverConfiguration;
    }

    [Trait("Category", "Smoke_ExternalSystemsHealth")]
    [Fact]
    public async Task Mailpit_ShouldBeUpAndRunning()
    {
        // Navigate to Mailpit URL
        HttpResponseMessage response;
        using (var client = new HttpClient())
        {
            if (!_serverConfiguration.MailpitCredentials.IsEmpty())
            {
                var byteArray = Encoding.ASCII.GetBytes($"{_serverConfiguration.MailpitCredentials.Username}:{_serverConfiguration.MailpitCredentials.Password}");
                client.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            }
            response = await client.GetAsync(_serverConfiguration.MailpitUrl);
        }
        // Check if the response is successful
        Assert.True(response.IsSuccessStatusCode, $"Failed to connect to Mailpit at {_serverConfiguration.MailpitUrl}, status code: {response.StatusCode}");
    }
}
