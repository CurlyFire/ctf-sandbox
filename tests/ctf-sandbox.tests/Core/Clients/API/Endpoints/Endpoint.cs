using Microsoft.AspNetCore.Mvc;

namespace ctf_sandbox.tests.Core.Clients.API.Endpoints;

public abstract class Endpoint
{
    protected JsonHttpClient<ValidationProblemDetails> JsonHttpClient {get;}

    protected Endpoint(JsonHttpClient<ValidationProblemDetails> jsonHttpClient)
    {
        JsonHttpClient = jsonHttpClient;
    }
}