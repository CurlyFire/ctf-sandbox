namespace ctf_sandbox.tests.Clients.ExternalSystems;

public class BannedWordsRealClient : HealthyHttpClient
{
    public BannedWordsRealClient(HttpClient client) : base(client)
    {
    }
}