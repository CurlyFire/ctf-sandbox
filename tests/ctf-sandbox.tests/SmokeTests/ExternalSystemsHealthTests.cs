using System.Net.Http.Headers;
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
        var password = config.GetValue<string>("AdminPassword", string.Empty);

        // Navigate to Mailpit URL
        HttpResponseMessage response;
        using (var client = new HttpClient())
        {
            if (!string.IsNullOrEmpty(password))
            {
                // If password is set, use Basic Authentication
                var username = "admin"; // Mailpit default username
                var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
                client.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            }
            response = await client.GetAsync(url);
        }
        // Check if the response is successful
        Assert.True(response.IsSuccessStatusCode, $"Failed to connect to Mailpit at {url}, status code: {response.StatusCode}");
    }
}
