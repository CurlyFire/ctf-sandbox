using ctf_sandbox.tests.Fixtures;

namespace ctf_sandbox.tests.Dsl;

public record SignInParameters
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    private SignInParameters()
    {
    }
    
    public static SignInParameters CreateWithDefaults(EnvironmentConfiguration config)
    {
        return new SignInParameters
        {
            UserName = config.WebServerCredentials.Username,
            Password = config.WebServerCredentials.Password
        };
    }
}
