using ctf_sandbox.tests.Utils;

namespace ctf_sandbox.tests.Fixtures;

/// <summary>Returns the test environment for the requested external system, creating each one lazily and once.</summary>
public class TestEnvironments : IDisposable
{
    private readonly Lazy<TestEnvironment> _real = new(() => new RealExternalSystemsTestEnvironment());
    private readonly Lazy<TestEnvironment> _stubbed = new(() => new StubbedExternalSystemsTestEnvironment());

    //TODO: Add support to control parallel execution of test environments
    public TestEnvironment For(ExternalSystems externalSystem) => externalSystem switch
    {
        ExternalSystems.Real => _real.Value,
        ExternalSystems.Stubbed => _stubbed.Value,
        _ => throw new ArgumentOutOfRangeException(nameof(externalSystem))
    };

    public void Dispose()
    {
        if (_real.IsValueCreated) _real.Value.Dispose();
        if (_stubbed.IsValueCreated) _stubbed.Value.Dispose();
    }
}
