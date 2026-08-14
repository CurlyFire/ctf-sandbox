namespace ctf_sandbox.tests.Core.Drivers.ExternalSystems;

public interface IEmailsDriver
{
    Task ActivateRegistrationSentTo(string email);
}
