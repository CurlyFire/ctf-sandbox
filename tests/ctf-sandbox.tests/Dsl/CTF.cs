using ctf_sandbox.tests.Drivers;
using ctf_sandbox.tests.Fixtures;

namespace ctf_sandbox.tests.Dsl;

public class CTF
{
    private readonly ICTFDriver _driver;
    private readonly EnvironmentConfiguration _configuration;
    private Action<SignInParameters> _noConfiguration;

    public CTF(ICTFDriver driver, EnvironmentConfiguration configuration)
    {
        _driver = driver;
        _configuration = configuration;
        _noConfiguration = _ => { };
    }

    public async Task<Emails> CheckEmails()
    {
        return await _driver.CheckEmails();
    }

    public async Task<bool> CreateAccount(string email, string password)
    {
        return await _driver.CreateAccount(email, password);
    }

    public async Task<CTF> SignIn()
    {
        return await SignIn(_noConfiguration);
    }

    public async Task<CTF> SignIn(Action<SignInParameters> configure)
    {
        var parameters = SignInParameters.CreateWithDefaults(_configuration);
        configure(parameters);
        return await SignIn(parameters);
    }

    private async Task<CTF> SignIn(SignInParameters parameters)
    {
        await _driver.SignIn(parameters.UserName, parameters.Password);
        return this;
    }

    public async Task CreateTeam(string teamName)
    {
        await _driver.CreateTeam(teamName);
    }

    public async Task ConfirmTeamIsAvailable(string randomTeamName)
    {
        Assert.True(await _driver.IsTeamVisible(randomTeamName));
    }

    public async Task ConfirmUserIsSignedIn(string email)
    {
        Assert.True(await _driver.IsUserSignedIn(email));
    }
}
