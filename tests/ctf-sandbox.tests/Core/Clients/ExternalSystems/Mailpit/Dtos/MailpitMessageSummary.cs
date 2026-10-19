namespace ctf_sandbox.tests.Core.Clients.ExternalSystems.Mailpit.Dtos;

public class MailpitMessageSummary
{
    public string ID { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public MailpitAddress From { get; set; } = new();
    public List<MailpitAddress> To { get; set; } = new();
    public DateTime Created { get; set; }
}
