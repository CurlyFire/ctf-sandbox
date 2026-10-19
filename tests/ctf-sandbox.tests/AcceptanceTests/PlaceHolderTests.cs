using ctf_sandbox.tests.Utils;

namespace ctf_sandbox.tests.AcceptanceTests;

public class PlaceHolderTests
{
    [Trait("Category", "Acceptance")]
    [Theory]
    [CombinatorialData]
    public void Test1(ExternalSystems externalSystem)
    {
        Assert.True(true);
    }
}

