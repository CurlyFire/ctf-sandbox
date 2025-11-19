namespace ctf_sandbox.tests.Dsl;

public class ExternalSystems
{
    private readonly Emails _emails;
    private readonly BannedWords _bannedWords;

    public ExternalSystems(Emails emails, BannedWords bannedWords)
    {
        _emails = emails;
        _bannedWords = bannedWords;
    }

    public Emails InteractWithEmails()
    {
        return _emails;
    }

    public BannedWords InteractWithBannedWords()
    {
        return _bannedWords;
    }

}
