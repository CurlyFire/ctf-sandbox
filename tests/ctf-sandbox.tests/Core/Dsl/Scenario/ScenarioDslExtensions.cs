using ctf_sandbox.tests.Utils;
using ctf_sandbox.tests.Fixtures;

namespace ctf_sandbox.tests.Core.Dsl.Scenario;

public static class ScenarioDslExtensions
{
    /// <summary>Starts a scenario using the default system channel.</summary>
    public static ScenarioDsl Scenario(this CTFFixture fixture)
        => new(fixture.InteractWithSystem());

    /// <summary>Starts a scenario that interacts with the system through the given channel.</summary>
    public static ScenarioDsl Scenario(this CTFFixture fixture, Channel channel)
        => new(fixture.InteractWithSystemThrough(channel));
}
