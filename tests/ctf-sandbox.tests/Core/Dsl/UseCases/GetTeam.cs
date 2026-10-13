using ctf_sandbox.Areas.CTF.Models;
using ctf_sandbox.tests.Core.Drivers.CTF;

namespace ctf_sandbox.tests.Core.Dsl.UseCases;

public class GetTeam : CTFUseCase<Team, GetTeamVerification>
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

    public override async Task<CTFUseCaseResult<Team, GetTeamVerification>> Execute()
    {
        var result = await Driver.GetTeam(_teamName);
        var mapped = result.IsFailure
            ? Result<Team, SystemError>.Failure(result.Error)
            : result.Value is null
                ? Result<Team, SystemError>.Failure(SystemError.Of($"Team '{_teamName}' was not found."))
                : Result<Team, SystemError>.Success(result.Value);

        return new CTFUseCaseResult<Team, GetTeamVerification>(
            mapped,
            Context,
            (response, ctx) => new GetTeamVerification(response, ctx));
    }
}
