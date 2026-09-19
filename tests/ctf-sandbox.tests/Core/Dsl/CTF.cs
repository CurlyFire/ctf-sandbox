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

    public CreateAccount CreateAccount() => _useCaseFactory.Create<CreateAccount>();
    
    public SignIn SignIn() => _useCaseFactory.Create<SignIn>();

    public CreateTeam CreateTeam() => _useCaseFactory.Create<CreateTeam>();

    public UpdateTeam UpdateTeam() => _useCaseFactory.Create<UpdateTeam>();

    public GetIpInfo GetIpInfo() => _useCaseFactory.Create<GetIpInfo>();

    public GoToCTF GoToCTF() => _useCaseFactory.Create<GoToCTF>();
}