using ctf_sandbox.Models;
using ctf_sandbox.tests.Core.Dsl.UseCases;

namespace ctf_sandbox.tests.Core.Dsl;

public class CTF
{
    private readonly UseCaseFactory _useCaseFactory;

    public CTF(UseCaseFactory useCaseFactory)
    {
        _useCaseFactory = useCaseFactory;
    }

    public async Task<bool> CreateAccount(string email, string password)
    {
        throw new NotImplementedException();
        //return await _driver.CreateAccount(email, password);
    }
    
    public SignIn SignIn() => _useCaseFactory.Create<SignIn>();

    public CreateTeam CreateTeam() => _useCaseFactory.Create<CreateTeam>();

    public UpdateTeam UpdateTeam() => _useCaseFactory.Create<UpdateTeam>();


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

    public GoToCTF GoToCTF() => _useCaseFactory.Create<GoToCTF>();
}