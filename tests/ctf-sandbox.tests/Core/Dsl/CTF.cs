using ctf_sandbox.Models;
using ctf_sandbox.tests.Core.Drivers.CTF;
using ctf_sandbox.tests.Core.Dsl.UseCases;
using ctf_sandbox.tests.Fixtures;

namespace ctf_sandbox.tests.Core.Dsl;

public class CTF
{
    private readonly ICTFDriver _driver;
    private readonly CTFConfiguration _configuration;
    private Action<SignInParameters> _noConfiguration;
    private readonly UseCaseContext _context;


    public CTF(ICTFDriver driver, CTFConfiguration configuration, UseCaseContext context)
    {
        _driver = driver;
        _configuration = configuration;
        _noConfiguration = _ => { };
        _context = context;
    }

    public async Task<bool> CreateAccount(string email, string password)
    {
        throw new NotImplementedException();
        //return await _driver.CreateAccount(email, password);
    }

    public UseCases.SignIn SignIn() => SignIn(_noConfiguration);

    public UseCases.SignIn SignIn(Action<SignInParameters> configure)
    {
        var parameters = SignInParameters.CreateWithDefaults(_configuration);
        configure(parameters);
        return new UseCases.SignIn(_driver, _context, parameters);
    }

    public async Task<string?> CreateTeam(string? teamName, uint memberCount = 4)
    {
        throw new NotImplementedException();
        //return await _driver.CreateTeam(teamName, memberCount);
    }

    public async Task UpdateTeam(string oldTeamName, string newTeamName, string? newDescription = null, uint? memberCount = null)
    {
        await _driver.UpdateTeam(oldTeamName, newTeamName, newDescription, memberCount);
    }

    public async Task ConfirmTeamIsAvailable(string teamName, uint? expectedMemberCount = null)
    {
        throw new NotImplementedException();

        // var team = await _driver.GetTeam(teamName);
        // Assert.NotNull(team);
        
        // if (expectedMemberCount.HasValue)
        // {
        //     Assert.Equal(expectedMemberCount.Value, team.MemberCount);
        // }
    }

    public async Task ConfirmTeamIsNotAvailable(string teamName, uint? unexpectedMemberCount = null)
    {
        throw new NotImplementedException();

        // var team = await _driver.GetTeam(teamName);
        
        // if (unexpectedMemberCount.HasValue)
        // {
        //     // Team should either not exist, or if it exists, should not have the specified member count
        //     Assert.True(team == null || team.MemberCount != unexpectedMemberCount.Value);
        // }
        // else
        // {
        //     // Team should not exist at all
        //     Assert.Null(team);
        // }
    }

    public async Task ConfirmUserIsSignedIn(string email)
    {
        throw new NotImplementedException();

        // await _driver.ConfirmUserIsSignedIn(email);
    }

    public async Task ConfirmUserIsSignedIn()
    {
        throw new NotImplementedException();

        // var parameters = SignInParameters.CreateWithDefaults(_configuration);        
        // await _driver.ConfirmUserIsSignedIn(parameters.UserName);
    }

    public async Task<IpInfo> GetIpInfo(string ipAddress)
    {
        throw new NotImplementedException();
        // return await _driver.GetIpInfo(ipAddress);
    }

    public GoToCTF GoToCTF() => new(_driver, _context);
}