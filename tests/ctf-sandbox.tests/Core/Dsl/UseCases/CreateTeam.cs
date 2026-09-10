using ctf_sandbox.Areas.CTF.Models;
using ctf_sandbox.tests.Core.Drivers.CTF;

namespace ctf_sandbox.tests.Core.Dsl.UseCases;

public class CreateTeam : CTFUseCase<Team, CreateTeamVerification>
{
    private readonly CreateTeamParameters _parameters;

    public CreateTeam(ICTFDriver driver, UseCaseContext context, CreateTeamParameters parameters) : base(driver, context)
    {
        _parameters = parameters;
    }

    public CreateTeam With(Action<CreateTeamParameters> configure)
    {
        configure(_parameters);
        return this;
    }

    public override async Task<CTFUseCaseResult<Team, CreateTeamVerification>> Execute()
    {
        var result = await Driver.CreateTeam(_parameters.TeamName, _parameters.MemberCount);
        return new CTFUseCaseResult<Team, CreateTeamVerification>(
            result,
            Context,
            (response, ctx) => new CreateTeamVerification(response, ctx));
    }
}

public record CreateTeamParameters
{
    public string TeamName { get; set; } = string.Empty;
    public uint MemberCount { get; set; } = 0;
    
}
