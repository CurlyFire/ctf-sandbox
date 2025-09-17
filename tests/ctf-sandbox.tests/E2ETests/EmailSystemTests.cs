namespace ctf_sandbox.tests.E2ETests;

public class EmailSystemTests : CTFDslTests
{
    [Trait("Category", "E2E")]
    [Fact]
    public async Task ShouldBeAbleToViewEmails()
    {
        var emailsDsl = await CTFDsl.CheckEmails();
        Assert.True(await emailsDsl.IsInboxAvailable());
    }
}

