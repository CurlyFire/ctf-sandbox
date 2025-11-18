namespace ctf_sandbox.tests.Dsl;

public class ExternalSystems
{
    private readonly Emails _emails;

    public ExternalSystems(Emails emails)
    {
        _emails = emails;
        
    }

    public Emails InteractWithEmails()
    {
        return _emails;
    }

}
