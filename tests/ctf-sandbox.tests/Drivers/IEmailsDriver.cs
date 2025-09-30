namespace ctf_sandbox.tests.Drivers;

public interface IEmailsDriver
{
    Task<bool> IsInboxAvailable();
    Task<bool> ConfirmRegistrationSentTo(string email);
}
