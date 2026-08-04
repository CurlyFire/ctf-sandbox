namespace ctf_sandbox.tests.Clients.ExternalSystems;

public class MailpitRealClient : HealthyHttpClient
{
    public MailpitRealClient(HttpClient client) : base(client)
    {
    }
}