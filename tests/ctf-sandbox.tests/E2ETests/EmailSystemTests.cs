using ctf_sandbox.tests.Fixtures;
using ctf_sandbox.tests.Utils;

namespace ctf_sandbox.tests.E2ETests;

public class EmailSystemTests : CTFTests
{
    public EmailSystemTests(EnvironmentFixture fixture) : base(fixture)
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