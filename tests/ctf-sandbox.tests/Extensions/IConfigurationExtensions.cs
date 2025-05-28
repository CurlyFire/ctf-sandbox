using Microsoft.Extensions.Configuration;

namespace ctf_sandbox.tests.Extensions;

public static class IConfigurationExtensions
{
    public static T GetRequiredValue<T>(this IConfiguration config, string key)
    {
        var value = config.GetValue<T>(key);

        if (value == null || value.Equals(default(T)))
        {
            throw new InvalidOperationException($"Missing required configuration: {key}");
        }

        return value;
    }

}
