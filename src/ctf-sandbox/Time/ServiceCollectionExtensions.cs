namespace ctf_sandbox.Time;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTimeProviderBasedOn(this IServiceCollection services, IConfiguration configuration)
    {
        var timeProviderOptionsSection = configuration.GetSection(TimeProviderOptions.ConfigurationSection);
        var timeProviderOptions = timeProviderOptionsSection.Get<TimeProviderOptions>() ?? new TimeProviderOptions();
        if (timeProviderOptions.Type == "system")
        {
            services.AddSingleton(TimeProvider.System);
        }
        else if (timeProviderOptions.Type == "http")
        {
            services.Configure<TimeProviderOptions>(timeProviderOptionsSection);
            services.AddHttpClient(HttpTimeProvider.HTTPClientName);
            services.AddSingleton<TimeProvider, HttpTimeProvider>();
        }
        else
        {
            throw new InvalidOperationException($"Unsupported TimeProvider type: {timeProviderOptions.Type}");
        }

        return services;
    }
}