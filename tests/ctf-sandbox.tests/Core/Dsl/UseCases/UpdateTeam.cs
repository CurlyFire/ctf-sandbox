using ctf_sandbox.Areas.CTF.Models;
using ctf_sandbox.tests.Core.Drivers.CTF;

namespace ctf_sandbox.tests.Core.Dsl.UseCases;

public class UpdateTeam : CTFUseCase<Team, UpdateTeamVerification>
{
    private readonly UpdateTeamParameters _parameters;

    public UpdateTeam(ICTFDriver driver, UseCaseContext context, UpdateTeamParameters parameters) : base(driver, context)
    {
        _parameters = parameters;
    }

    public UpdateTeam With(Action<UpdateTeamParameters> configure)
    {
        configure(_parameters);
        return this;
    }

    public override async Task<CTFUseCaseResult<Team, UpdateTeamVerification>> Execute()
    {
        var result = await Driver.UpdateTeam(_parameters.OriginalTeamName, _parameters.TeamName, _parameters.Description, _parameters.MemberCount);
        return new CTFUseCaseResult<Team, UpdateTeamVerification>(
            result,
            Context,
            (response, ctx) => new UpdateTeamVerification(response, ctx));
    }
}

public record UpdateTeamParameters
{
    public string OriginalTeamName { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;
    public string? Description { get; set; } = null;
    public uint MemberCount { get; set; } = 0;
    
}
