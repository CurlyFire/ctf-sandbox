using ctf_sandbox.tests.Core.Clients.ExternalSystems;

namespace ctf_sandbox.tests.Core.Drivers.ExternalSystems;

public class APIEmailsDriver : IEmailsDriver
{

    private readonly MailpitRealClient _client;

    public APIEmailsDriver(MailpitRealClient client)
    {
        _client = client;
    }

    public async Task ActivateRegistrationSentTo(string email)
    {
        await _client.ActivateRegistrationSentTo(email);
    }
}