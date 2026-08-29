using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ctf_sandbox.tests.Core.Clients;

public class JsonHttpClient<E> : IDisposable
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false) }
    };

    private readonly HttpClient _httpClient;
    private bool _disposed;

    public JsonHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _httpClient?.Dispose();
            }
            _disposed = true;
        }
    }

    public async Task<Result<T, E>> GetAsync<T>(string path, string? jwt = null)
        => await GetResultOrFailureAsync<T>(await DoGetAsync(path, jwt));

    public async Task<Result<VoidValue, E>> GetAsync(string path, string? jwt = null)
        => await GetResultOrFailureAsync<VoidValue>(await DoGetAsync(path, jwt));

    public async Task<Result<T, E>> PostAsync<T>(string path, object request, string? jwt = null)
        => await GetResultOrFailureAsync<T>(await DoPostAsync(path, request, jwt));

    public async Task<Result<VoidValue, E>> PostAsync(string path, object request, string? jwt = null)
        => await GetResultOrFailureAsync<VoidValue>(await DoPostAsync(path, request, jwt));

    public async Task<Result<VoidValue, E>> PostAsync(string path, string? jwt = null)
        => await GetResultOrFailureAsync<VoidValue>(await DoPostAsync(path, jwt));

    public async Task<Result<T, E>> PutAsync<T>(string path, object request, string? jwt = null)
        => await GetResultOrFailureAsync<T>(await DoPutAsync(path, request, jwt));

    public async Task<Result<VoidValue, E>> PutAsync(string path, object request, string? jwt = null)
        => await GetResultOrFailureAsync<VoidValue>(await DoPutAsync(path, request, jwt));

    public async Task<Result<T, E>> DeleteAsync<T>(string path, string? jwt = null)
        => await GetResultOrFailureAsync<T>(await DoDeleteAsync(path, jwt));

    public async Task<Result<VoidValue, E>> DeleteAsync(string path, string? jwt = null)
        => await GetResultOrFailureAsync<VoidValue>(await DoDeleteAsync(path, jwt));

    private async Task<HttpResponseMessage> DoGetAsync(string path, string? jwt = null)
    {
        var uri = GetUri(path);
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, uri);
        return await SendRequest(httpRequest, jwt);
    }

    #region Helpers

    private Uri GetUri(string path)
    {
        return new Uri(_httpClient.BaseAddress!, path);
    }

    private async Task<HttpResponseMessage> DoPostAsync(string path, object request, string? jwt = null)
    {
        var uri = GetUri(path);
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, uri)
        {
            Content = JsonContent.Create(request, options: _jsonOptions)
        };
        return await SendRequest(httpRequest, jwt);
    }

    private async Task<HttpResponseMessage> DoPostAsync(string path, string? jwt = null)
    {
        var uri = GetUri(path);
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, uri)
        {
            Content = JsonContent.Create(new { }, options: _jsonOptions)
        };
        return await SendRequest(httpRequest, jwt);
    }

    private async Task<HttpResponseMessage> DoPutAsync(string path, object request, string? jwt = null)
    {
        var uri = GetUri(path);
        var httpRequest = new HttpRequestMessage(HttpMethod.Put, uri)
        {
            Content = JsonContent.Create(new { }, options: _jsonOptions)
        };
        return await SendRequest(httpRequest, jwt);
    }

    private async Task<HttpResponseMessage> DoDeleteAsync(string path, string? jwt = null)
    {
        var uri = GetUri(path);
        var httpRequest = new HttpRequestMessage(HttpMethod.Delete, uri);
        return await SendRequest(httpRequest, jwt);
    }

    private Task<HttpResponseMessage> SendRequest(HttpRequestMessage httpRequest, string? jwt = null)
        => _httpClient.SendAsync(httpRequest);

    private static async Task<T> ReadResponseAsync<T>(HttpResponseMessage httpResponse, JsonSerializerOptions jsonOptions)
    {
        var response = await httpResponse.Content.ReadFromJsonAsync<T>(jsonOptions);
        return response ?? throw new InvalidOperationException("Response content is null.");
    }

    private async Task<Result<T, E>> GetResultOrFailureAsync<T>(HttpResponseMessage httpResponse)
    {
        if (!httpResponse.IsSuccessStatusCode)
        {
            var error = await ReadResponseAsync<E>(httpResponse, _jsonOptions);
            return Result<T, E>.Failure(error);
        }

        if (typeof(T) == typeof(VoidValue) || httpResponse.StatusCode == HttpStatusCode.NoContent)
        {
            return Result<T, E>.Success(default!);
        }

        var response = await ReadResponseAsync<T>(httpResponse, _jsonOptions);
        return Result<T, E>.Success(response);
    }

    #endregion
}
