using System.Net.Http.Json;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace ctf_sandbox.tests.Core.Clients.API.Endpoints;

public static class HttpResponseMessageExtensions
{
    public static async Task<ValidationProblemDetails?> GetValidationProblemDetails(this HttpResponseMessage response)
    {
        if (response.Content.Headers.ContentType?.MediaType == MediaTypeNames.Application.ProblemJson)
        {
            var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            if (problemDetails == null)
            {
                throw new InvalidOperationException("Could not deserialize response content as ValidationProblemDetails.");
            }
            return problemDetails;
        }
        return null;
    }
}