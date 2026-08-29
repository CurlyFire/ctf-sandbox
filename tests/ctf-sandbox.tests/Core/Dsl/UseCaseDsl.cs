namespace ctf_sandbox.tests.Core.Dsl;

public class UseCaseDsl
{
    public CTF CTF { get; }
    public Emails Emails { get; }
    public BannedWords BannedWords { get; }

    public UseCaseDsl(CTF ctf, Emails emails, BannedWords bannedWords)
    {
        CTF = ctf;
        Emails = emails;
        BannedWords = bannedWords;
    }
}