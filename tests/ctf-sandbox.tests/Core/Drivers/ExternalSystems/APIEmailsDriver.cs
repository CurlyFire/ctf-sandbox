using ctf_sandbox.tests.Core.Clients.ExternalSystems;
using ctf_sandbox.tests.Core;

namespace ctf_sandbox.tests.Core.Drivers.ExternalSystems;

public class APIEmailsDriver : IEmailsDriver
{

    private readonly MailpitRealClient _client;

    public APIEmailsDriver(MailpitRealClient client)
    {
        _client = client;
    }

    public async Task<Result<VoidValue, SystemError>> GoToMailpit()
    {
        var isHealthy = await _client.IsHealthy();
        return isHealthy
            ? Result.Success<SystemError>()
            : Result.Failure<SystemError>(SystemError.Of("Mailpit service is not healthy"));
    }

    public async Task<Result<VoidValue, SystemError>> ActivateRegistrationSentTo(string email)
    {
        try
        {
            await _client.ActivateRegistrationSentTo(email);
            return Result.Success<SystemError>();
        }
        catch (Exception exception)
        {
            return Result.Failure<SystemError>(SystemError.Of(exception.Message));
        }
    }
}