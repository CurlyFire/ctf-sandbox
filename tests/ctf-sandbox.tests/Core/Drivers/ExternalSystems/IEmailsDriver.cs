using ctf_sandbox.tests.Core;

namespace ctf_sandbox.tests.Core.Drivers.ExternalSystems;

public interface IEmailsDriver
{
    Task<Result<VoidValue, SystemError>> GoToMailpit();
    Task ActivateRegistrationSentTo(string email);
}
