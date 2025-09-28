using ctf_sandbox.tests.Fixture.Drivers;

namespace ctf_sandbox.tests.Fixture.Dsl;

public class CTFDsl
{
    private readonly ICTFDriver _driver;

    public CTFDsl(ICTFDriver driver)
    {
        _driver = driver;
    }

    public async Task<EmailsDsl> CheckEmails()
    {
        return await _driver.CheckEmails();
    }

    public async Task<bool> CreateAccount(string email, string password)
    {
        return await _driver.CreateAccount(email, password);
    }

    public async Task<CTFDsl> SignIn(string email, string password)
    {
        await _driver.SignIn(email, password);
        return this;
    }

    public async Task CreateTeam(string teamName)
    {
        await _driver.CreateTeam(teamName);
    }

    public async Task<bool> IsTeamVisible(string teamName)
    {
        return await _driver.IsTeamVisible(teamName);
    }
}
