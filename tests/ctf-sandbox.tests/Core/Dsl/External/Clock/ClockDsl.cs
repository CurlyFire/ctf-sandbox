using ctf_sandbox.tests.Core.Dsl.UseCases;
using ctf_sandbox.tests.Core.Dsl.External.Clock.UseCases;
using ctf_sandbox.tests.Core.Drivers.ExternalSystems;

namespace ctf_sandbox.tests.Core.Dsl.External.Clock;

public class ClockDsl
{
    private readonly UseCaseFactory<IClockDriver> _useCaseFactory;

    public ClockDsl(UseCaseFactory<IClockDriver> useCaseFactory)
    {
        _useCaseFactory = useCaseFactory;
    }

    public GoToClock GoToClock() => _useCaseFactory.Create<GoToClock>();
}
