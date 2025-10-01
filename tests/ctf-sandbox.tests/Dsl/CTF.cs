using ctf_sandbox.tests.Drivers;
using ctf_sandbox.tests.Fixtures.Utils;

namespace ctf_sandbox.tests.Dsl;

public class CTF
{
    private readonly ICTFDriver _driver;
    private readonly ServerConfiguration _configuration;

    public CTF(ICTFDriver driver, ServerConfiguration configuration)
    {
        _driver = driver;
        _configuration = configuration;
    }

    public async Task<Emails> CheckEmails()
    {
        return await _driver.CheckEmails();
    }

    public async Task<bool> CreateAccount(string email, string password)
    {
        return await _driver.CreateAccount(email, password);
    }

    public async Task<CTF> SignIn(string email, string password)
    {
        await _driver.SignIn(email, password);
        return this;
    }

    public async Task<CTF> SignIn()
    {
        await _driver.SignIn(_configuration.WebServerCredentials.Username,
            _configuration.WebServerCredentials.Password);
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
}
