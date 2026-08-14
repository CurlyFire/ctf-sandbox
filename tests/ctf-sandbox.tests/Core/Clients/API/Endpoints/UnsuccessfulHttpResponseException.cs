using Microsoft.AspNetCore.Mvc;

namespace ctf_sandbox.tests.Core.Clients.API.Endpoints;

public class UnsuccessfulHttpResponseException : Exception
{
    public HttpResponseMessage Response { get; }

    public UnsuccessfulHttpResponseException(HttpResponseMessage response)
        : base($"Request failed with status code: {response.StatusCode}")
    {
        Response = response;
    }

    public override string ToString()
    {
        return base.ToString();
    }
}