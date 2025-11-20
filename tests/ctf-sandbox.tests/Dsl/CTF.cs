using ctf_sandbox.Models;
using ctf_sandbox.tests.Drivers.CTF;
using ctf_sandbox.tests.Fixtures;

namespace ctf_sandbox.tests.Dsl;

public class CTF
{
    private readonly ICTFDriver _driver;
    private readonly CTFConfiguration _configuration;
    private Action<SignInParameters> _noConfiguration;


    public CTF(ICTFDriver driver, CTFConfiguration configuration)
    {
        _driver = driver;
        _configuration = configuration;
        _noConfiguration = _ => { };
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

    public async Task<string?> CreateTeam(string? teamName, uint memberCount = 4)
    {
        return await _driver.CreateTeam(teamName, memberCount);
    }

    public async Task UpdateTeam(string oldTeamName, string newTeamName, string? newDescription = null, uint? memberCount = null)
    {
        await _driver.UpdateTeam(oldTeamName, newTeamName, newDescription, memberCount);
    }

    public async Task ConfirmTeamIsAvailable(string teamName, uint? expectedMemberCount = null)
    {
        var team = await _driver.GetTeam(teamName);
        Assert.NotNull(team);
        
        if (expectedMemberCount.HasValue)
        {
            Assert.Equal(expectedMemberCount.Value, team.MemberCount);
        }
    }

    public async Task ConfirmTeamIsNotAvailable(string teamName, uint? unexpectedMemberCount = null)
    {
        var team = await _driver.GetTeam(teamName);
        
        if (unexpectedMemberCount.HasValue)
        {
            // Team should either not exist, or if it exists, should not have the specified member count
            Assert.True(team == null || team.MemberCount != unexpectedMemberCount.Value);
        }
        else
        {
            // Team should not exist at all
            Assert.Null(team);
        }
    }

    public async Task ConfirmUserIsSignedIn(string email)
    {
        Assert.True(await _driver.IsUserSignedIn(email));
    }

    public async Task<IpInfo> GetIpInfo(string ipAddress)
    {
        return await _driver.GetIpInfo(ipAddress);
    }
}
