using System.Net.Http.Json;

namespace ctf_sandbox.tests.Clients.API.Endpoints;

public abstract class Endpoint
{
    private readonly HttpClient _httpClient;

    protected HttpClient HttpClient => _httpClient;

    protected Endpoint(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    protected async Task<T> GetAsyncAndEnsureSuccess<T>(string url)
    {
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>() ?? throw new InvalidOperationException("Response content is null.");
    }

    protected async Task PostAsyncAndEnsureSuccess<TRequest>(string url, TRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(url, request);
        response.EnsureSuccessStatusCode();
    }

    protected async Task<TResponse> PostAsyncAndEnsureSuccess<TRequest, TResponse>(string url, TRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(url, request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>() ?? throw new InvalidOperationException("Response content is null.");
    }

    protected async Task PutAsyncAndEnsureSuccess<TRequest>(string url, TRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync(url, request);
        response.EnsureSuccessStatusCode();
    }
}