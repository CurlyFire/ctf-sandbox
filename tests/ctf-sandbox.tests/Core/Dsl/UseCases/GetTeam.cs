using ctf_sandbox.tests.Core.Drivers.CTF.Dtos;
using ctf_sandbox.tests.Core.Drivers.CTF;

namespace ctf_sandbox.tests.Core.Dsl.UseCases;

public class GetTeam : CTFUseCase<GetTeamResponse, GetTeamVerification>
{
    private string _teamName = string.Empty;

    public GetTeam(ICTFDriver driver, UseCaseContext context) : base(driver, context)
    {
    }

    public GetTeam WithTeamName(string teamName)
    {
        _teamName = teamName;
        return this;
    }

    public override async Task<CTFUseCaseResult<GetTeamResponse, GetTeamVerification>> Execute()
    {
        var teamName = Context.GetParamValue(_teamName)!;
        var result = await Driver.GetTeam(teamName);
        var mapped = result.IsFailure
            ? Result<GetTeamResponse, SystemError>.Failure(result.Error)
            : result.Value is null
                ? Result<GetTeamResponse, SystemError>.Failure(SystemError.Of($"Team '{teamName}' was not found."))
                : Result<GetTeamResponse, SystemError>.Success(result.Value);

        return new CTFUseCaseResult<GetTeamResponse, GetTeamVerification>(
            mapped,
            Context,
            (response, ctx) => new GetTeamVerification(response, ctx));
    }
}
