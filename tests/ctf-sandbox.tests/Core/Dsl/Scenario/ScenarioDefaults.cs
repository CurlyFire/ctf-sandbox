namespace ctf_sandbox.tests.Core.Dsl.Scenario;

/// <summary>Values used when a scenario does not specify them.</summary>
public static class ScenarioDefaults
{
    public const uint DefaultMemberCount = 4;

    public static string NewTeamName() => $"team_{Guid.NewGuid()}";
}
