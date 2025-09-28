namespace ctf_sandbox.tests.Fixtures.Drivers;

public interface IEmailsDriver
{
    Task<bool> IsInboxAvailable();
    Task<bool> ConfirmRegistrationSentTo(string email);
}
