using ctf_sandbox.tests.Core;
using ctf_sandbox.tests.Core.Dsl;
using CtfSandbox.Tests.Core.Drivers.ExternalSystems;

namespace ctf_sandbox.tests.Core.Dsl.External.Clock.UseCases;

public class GoToClock : ClockUseCase<VoidValue, VoidVerification>
{
    public GoToClock(IClockDriver driver, UseCaseContext context) : base(driver, context)
    {
    }

    public override async Task<ClockUseCaseResult<VoidValue, VoidVerification>> Execute()
    {
        var result = await Driver.GoToClock();
        return new ClockUseCaseResult<VoidValue, VoidVerification>(
            result,
            Context,
            (response, ctx) => new VoidVerification(response, ctx));
    }
}
