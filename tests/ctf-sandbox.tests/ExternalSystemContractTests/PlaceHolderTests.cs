using ctf_sandbox.tests.Utils;

namespace ctf_sandbox.tests.ExternalSystemContractTests;

public class PlaceHolderTests
{
    [Trait("Category", "ExternalSystemContract")]
    [Theory, CombinatorialData]
    public void Test1(ExternalSystems externalSystem)
    {
        Assert.True(true);
    }
}
