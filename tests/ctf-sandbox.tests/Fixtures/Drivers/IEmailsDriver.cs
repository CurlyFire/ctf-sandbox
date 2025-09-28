namespace ctf_sandbox.tests.Fixture.Drivers;

public interface IEmailsDriver
{
    Task<bool> IsInboxAvailable();
    Task<bool> ConfirmRegistrationSentTo(string email);
}
