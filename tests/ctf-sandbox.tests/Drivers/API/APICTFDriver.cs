using ctf_sandbox.tests.Dsl;

namespace ctf_sandbox.tests.Drivers.API;

public class APICTFDriver : ICTFDriver
{
    public Task<Emails> CheckEmails()
    {
        throw new NotImplementedException();
    }

    public Task<bool> CreateAccount(string email, string password)
    {
        throw new NotImplementedException();
    }

    public Task CreateTeam(string teamName)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsTeamVisible(string teamName)
    {
        throw new NotImplementedException();
    }

    public Task SignIn(string email, string password)
    {
        throw new NotImplementedException();
    }
}
