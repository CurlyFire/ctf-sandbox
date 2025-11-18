namespace ctf_sandbox.tests.Drivers.ExternalSystems;

public interface IEmailsDriver
{
    Task ActivateRegistrationSentTo(string email);
}
