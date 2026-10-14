using ctf_sandbox.tests.Core.Dsl.Scenario;
using ctf_sandbox.tests.Fixtures;
using ctf_sandbox.tests.Utils;

namespace ctf_sandbox.tests.E2ETests;

[Collection(CTFTestCollection.Name)]
public class IpInfoTests
{
    private readonly TestEnvironment _testEnvironment;

    public IpInfoTests(TestEnvironments testEnvironments)
    {
        _testEnvironment = testEnvironments.For(ExternalSystem.Real);
    }

    [Trait("Category", "E2E")]
    [Theory, CombinatorialData]
    public async Task ShouldBeAbleToGetIpInfo(Channel channel)
    {
        await _testEnvironment.Scenario(channel)
            .Given().SignedIn()
            .When().GetIpInfo().WithIpAddress("8.8.8.8")
            .Then().ShouldSucceed()
            .And().IpInfo()
                .HasIp("8.8.8.8")
                .HasLocationData();
    }
}
