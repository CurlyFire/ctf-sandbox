namespace ctf_sandbox.tests.E2ETests;

public class EmailSystemTests : IClassFixture<CTFFixture>
{
    private readonly CTFFixture _fixture;

    public EmailSystemTests(CTFFixture fixture)
    {
        _fixture = fixture;
    }

    [Trait("Category", "E2E")]
    [Theory]
    [Channel(Channel.UI)]
    public async Task ShouldBeAbleToViewEmails(Channel channel)
    {
        var ctfDsl = _fixture.GetDsl(channel);
        var emailsDsl = await ctfDsl.CheckEmails();
        Assert.True(await emailsDsl.IsInboxAvailable());
    }
}

