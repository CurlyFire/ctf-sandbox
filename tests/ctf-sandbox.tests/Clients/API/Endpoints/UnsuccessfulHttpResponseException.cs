using Microsoft.AspNetCore.Mvc;

namespace ctf_sandbox.tests.Clients.API.Endpoints;

public class UnsuccessfulHttpResponseException : Exception
{
    public HttpResponseMessage Response { get; }

    public UnsuccessfulHttpResponseException(HttpResponseMessage response)
        : base($"Request failed with status code: {response.StatusCode}")
    {
        Response = response;
    }
}