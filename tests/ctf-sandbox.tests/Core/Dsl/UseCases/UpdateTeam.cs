using ctf_sandbox.tests.Core.Drivers.CTF.Dtos;
using ctf_sandbox.tests.Core.Drivers.CTF;

namespace ctf_sandbox.tests.Core.Dsl.UseCases;

public class UpdateTeam : CTFUseCase<GetTeamResponse, UpdateTeamVerification>
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

    public override async Task<CTFUseCaseResult<GetTeamResponse, UpdateTeamVerification>> Execute()
    {
        var result = await Driver.UpdateTeam(Context.GetParamValue(_parameters.OriginalTeamName)!, Context.GetParamValue(_parameters.TeamName)!, _parameters.Description, _parameters.MemberCount);
        return new CTFUseCaseResult<GetTeamResponse, UpdateTeamVerification>(
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
