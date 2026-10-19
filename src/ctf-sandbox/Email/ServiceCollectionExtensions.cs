namespace ctf_sandbox.Email;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSmtpClientBasedOn(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(SmtpClientOptions.ConfigurationSection);
        var options = section.Get<SmtpClientOptions>() ?? new SmtpClientOptions();
        services.Configure<SmtpClientOptions>(section);

        if (options.Type == "smtp")
        {
            services.AddTransient<ISmtpClient, RealSmtpClient>();
        }
        else if (options.Type == "http")
        {
            services.AddHttpClient(HttpSmtpClient.HTTPClientName);
            services.AddTransient<ISmtpClient, HttpSmtpClient>();
        }
        else
        {
            throw new InvalidOperationException($"Unsupported SmtpClient type: {options.Type}");
        }

        return services;
    }
}
