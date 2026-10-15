namespace ctf_sandbox.tests.Core.Dsl.Scenario.Then;

public class ThenSuccessAnd<TSuccessResponse, TSuccessVerification>
    where TSuccessVerification : ResponseVerification<TSuccessResponse>
{
    private readonly ThenStage<TSuccessResponse, TSuccessVerification> _stage;

    internal ThenSuccessAnd(ThenStage<TSuccessResponse, TSuccessVerification> stage)
    {
        _stage = stage;
    }

    /// <summary>Verifies the response returned by the When operation.</summary>
    public ThenResponse<TSuccessResponse, TSuccessVerification> Response(Action<TSuccessVerification> verification)
        => new(_stage, verification);

    /// <summary>Verifies the IP info returned by a GetIpInfo operation.</summary>
    public ThenIpInfo<TSuccessResponse, TSuccessVerification> IpInfo() => new(_stage);

    /// <summary>Verifies the state of a team by reading it back from the system.</summary>
    public ThenTeam<TSuccessResponse, TSuccessVerification> Team(string teamName) => new(_stage, teamName);
}
