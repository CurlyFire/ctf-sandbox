using ctf_sandbox.tests.Fixtures.Utils;
using ctf_sandbox.tests.Fixtures;

namespace ctf_sandbox.tests.E2ETests;

public class EmailSystemTests : IClassFixture<DSLFixture>
{
    private readonly DSLFixture _fixture;

    public EmailSystemTests(DSLFixture fixture)
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

