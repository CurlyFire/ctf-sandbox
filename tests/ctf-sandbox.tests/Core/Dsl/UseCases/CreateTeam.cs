using ctf_sandbox.tests.Core.Drivers.CTF.Dtos;
using ctf_sandbox.tests.Core.Drivers.CTF;

namespace ctf_sandbox.tests.Core.Dsl.UseCases;

public class CreateTeam : CTFUseCase<GetTeamResponse, CreateTeamVerification>
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

    public override async Task<CTFUseCaseResult<GetTeamResponse, CreateTeamVerification>> Execute()
    {
        var teamName = Context.GetParamValue(_parameters.TeamName)!;
        var result = _parameters.RawMemberCount is null
            ? await Driver.CreateTeam(teamName, _parameters.MemberCount)
            : await Driver.CreateTeam(teamName, _parameters.RawMemberCount);
        return new CTFUseCaseResult<GetTeamResponse, CreateTeamVerification>(
            result,
            Context,
            (response, ctx) => new CreateTeamVerification(response, ctx));
    }
}

public record CreateTeamParameters
{
    public string TeamName { get; set; } = string.Empty;
    public uint MemberCount { get; set; } = 0;
    public string? RawMemberCount { get; set; }
    
}
