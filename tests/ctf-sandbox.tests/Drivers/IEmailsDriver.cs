namespace ctf_sandbox.tests.Drivers;

public interface IEmailsDriver
{
    Task ActivateRegistrationSentTo(string email);
}
