using ctf_sandbox.tests.Fixtures.Utils;
using ctf_sandbox.tests.Fixtures;

namespace ctf_sandbox.tests.E2ETests;

public class EmailSystemTests : CTFTests
{
    public EmailSystemTests(ServerFixture fixture) : base(fixture)
    {
    }

    [Trait("Category", "E2E")]
    [Theory]
    [Channel(Channel.UI)]
    public async Task ShouldBeAbleToViewEmails(Channel channel)
    {
        var ctf = InteractWithCTFThrough(channel);

        var emails = await ctf.CheckEmails();

        await emails.ConfirmInboxIsAvailable();
    }
}