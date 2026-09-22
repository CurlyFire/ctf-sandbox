using ctf_sandbox.tests.Core.Dsl.External.BannedWords;
using ctf_sandbox.tests.Core.Dsl.External.Emails;
using CtfSandbox.Tests.Core.Dsl.External.IpInfo;

namespace ctf_sandbox.tests.Core.Dsl;

public class ExternalSystemsDsl
{
    public EmailsDsl Emails { get; }
    public BannedWordsDsl BannedWords { get; }
    public IpInfoDsl IpInfo { get; }

    public ExternalSystemsDsl(EmailsDsl emails, BannedWordsDsl bannedWords, IpInfoDsl ipInfo)
    {
        Emails = emails;
        BannedWords = bannedWords;
        IpInfo = ipInfo;
    }

}
