using Microsoft.Extensions.DependencyInjection;
using RestEase;

namespace ctf_sandbox.tests.Extensions;

public static class RestEaseServiceCollectionExtensions
{
    /// <summary>
    /// Like AddHttpClient&lt;TClient&gt;, but TClient's constructor additionally receives a RestEase
    /// TApi built on top of the same configured HttpClient.
    /// </summary>
    public static IHttpClientBuilder AddRestEaseClient<TClient, TApi>(
        this IServiceCollection services, Action<HttpClient> configureClient)
        where TClient : class
        where TApi : class
    {
        return services
            .AddHttpClient(typeof(TClient).Name, configureClient)
            .AddTypedClient<TClient>((httpClient, sp) =>
                ActivatorUtilities.CreateInstance<TClient>(sp, httpClient, new RestClient(httpClient).For<TApi>()));
    }

    public static IHttpClientBuilder AddRestEaseClient<TClient, TApi>(
        this IServiceCollection services, Func<string> baseUrl)
        where TClient : class
        where TApi : class
    {
        return services.AddRestEaseClient<TClient, TApi>(c => c.BaseAddress = new Uri(baseUrl()));
    }
}
