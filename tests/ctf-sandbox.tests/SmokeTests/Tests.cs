namespace ctf_sandbox.tests.SmokeTests;

public class Tests : CTFSandboxPageTest
{

    [Trait("Category", "Smoke")]
    [Fact]
    public async Task Test1()
    {
        var response = await Page.GotoAsync(string.Empty);
    }
}