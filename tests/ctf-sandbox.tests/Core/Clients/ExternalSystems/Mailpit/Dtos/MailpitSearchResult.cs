namespace ctf_sandbox.tests.Core.Clients.ExternalSystems.Mailpit.Dtos;

public class MailpitSearchResult
{
    public int Total { get; set; }
    public int Count { get; set; }
    public List<MailpitMessageSummary> Messages { get; set; } = new();
}
