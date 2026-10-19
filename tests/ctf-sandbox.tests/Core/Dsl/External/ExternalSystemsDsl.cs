using ctf_sandbox.tests.Core.Dsl.External.BannedWords;
using ctf_sandbox.tests.Core.Dsl.External.Emails;
using ctf_sandbox.tests.Core.Dsl.External.Clock;
using ctf_sandbox.tests.Core.Dsl.External.IpInfo;

namespace ctf_sandbox.tests.Core.Dsl;

public class ExternalSystemsDsl
{
    public EmailsDsl Emails { get; }
    public BannedWordsDsl BannedWords { get; }
    public IpInfoDsl IpInfo { get; }
    public ClockDsl Clock { get; }

    public ExternalSystemsDsl(EmailsDsl emails, BannedWordsDsl bannedWords, IpInfoDsl ipInfo, ClockDsl clock)
    {
        Emails = emails;
        BannedWords = bannedWords;
        IpInfo = ipInfo;
        Clock = clock;
    }

}
