using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ctf_sandbox.tests.Core.Clients.API.Endpoints;

public abstract class Endpoint
{
    private readonly HttpClient _httpClient;

    protected HttpClient HttpClient => _httpClient;

    protected Endpoint(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    protected async Task<T> GetAsyncAndEnsureSuccess<T>(string url, string? jwt = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        return await SendAsync<T>(request, jwt);
    }

    protected async Task PostAsyncAndEnsureSuccess<TRequest>(string url, TRequest body, string? jwt = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = JsonContent.Create(body) };
        await SendAsync(request, jwt);
    }

    protected async Task<TResponse> PostAsyncAndEnsureSuccess<TRequest, TResponse>(string url, TRequest body, string? jwt = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = JsonContent.Create(body) };
        return await SendAsync<TResponse>(request, jwt);
    }

    protected async Task PutAsyncAndEnsureSuccess<TRequest>(string url, TRequest body, string? jwt = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, url) { Content = JsonContent.Create(body) };
        await SendAsync(request, jwt);
    }
    

    private async Task SendAsync(HttpRequestMessage request, string? jwt)
    {
        await SendAndEnsureSuccessAsync(request, jwt);
    }

    private async Task<T> SendAsync<T>(HttpRequestMessage request, string? jwt)
    {
        var response = await SendAndEnsureSuccessAsync(request, jwt);

        var mediaType = response.Content.Headers.ContentType?.MediaType;

        if (mediaType != null && mediaType.Contains("json", StringComparison.OrdinalIgnoreCase))
            return await response.Content.ReadFromJsonAsync<T>() ?? throw new InvalidOperationException("Response content is null.");

        if (typeof(T) == typeof(string))
            return (T)(object)await response.Content.ReadAsStringAsync();

        throw new InvalidOperationException($"Cannot deserialize non-JSON response as {typeof(T).Name}.");
    }

    private async Task<HttpResponseMessage> SendAndEnsureSuccessAsync(HttpRequestMessage request, string? jwt)
    {
        if (jwt != null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
            return response;
        else
            throw new UnsuccessfulHttpResponseException(response);
    }
}