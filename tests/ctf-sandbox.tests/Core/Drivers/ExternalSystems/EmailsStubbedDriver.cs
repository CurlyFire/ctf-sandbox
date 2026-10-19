using ctf_sandbox.tests.Core.Clients.ExternalSystems.Mailpit;

namespace ctf_sandbox.tests.Core.Drivers.ExternalSystems;

public class EmailsStubbedDriver : IEmailsDriver
{
    private readonly MailpitStubbedClient _client;

    public EmailsStubbedDriver(MailpitStubbedClient client)
    {
        _client = client;
    }

    public Task<Result<VoidValue, SystemError>> GoToMailpit() => _client.CheckHealth();

    public async Task<Result<VoidValue, SystemError>> ActivateRegistrationSentTo(string email)
    {
        try
        {
            await _client.SetupRegistrationEmailSentTo(email);
            await _client.ActivateRegistrationSentTo(email);
            return Result.Success<SystemError>();
        }
        catch (Exception exception)
        {
            return Result.Failure<SystemError>(SystemError.Of(exception.Message));
        }
    }
}
