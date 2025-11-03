using ctf_sandbox.tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace ctf_sandbox.tests;

public class HttpTests : EnvironmentTests
{
    private const string CTFHttpClientName = "ctf";
    private const string EmailsHttpClientName = "emails";

    private HttpClient? _httpClient;

    public HttpTests(EnvironmentFixture fixture) : base(fixture)
    {
    }

    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.AddHttpClient(CTFHttpClientName, ConfigureCTFHttpClient);
        services.AddHttpClient(EmailsHttpClientName, ConfigureEmailsHttpClient);
    }

    public override void Configure(IServiceProvider serviceProvider)
    {
        base.Configure(serviceProvider);
        _httpClient = serviceProvider.GetRequiredService<HttpClient>();
    }

    protected void ConfigureCTFHttpClient(HttpClient httpClient)
    {
        httpClient.BaseAddress = new Uri(EnvironmentFixture.Configuration.WebServerUrl + "/api/");
    }

    protected void ConfigureEmailsHttpClient(HttpClient httpClient)
    {
        httpClient.BaseAddress = new Uri(EnvironmentFixture.Configuration.MailpitUrl + "/api/v1/");
    }

    public HttpClient GetCTFHttpClient()
    {
        if (_httpClient == null)
        {
            throw new InvalidOperationException("HttpClient has not been initialized. Ensure Configure has been called.");
        }

        return _httpClient;
    }
}
