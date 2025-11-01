using System.Text.Json;
using ctf_sandbox.Areas.CTF.Models;
using Microsoft.Extensions.Options;

namespace ctf_sandbox.Services;

public interface IIpInfoService
{
    Task<IpInfo?> GetIpInfoAsync(string ipAddress);
}

public class IpInfoService : IIpInfoService
{
    private readonly HttpClient _httpClient;
    private readonly IpInfoOptions _options;

    public IpInfoService(HttpClient httpClient, IOptions<IpInfoOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<IpInfo?> GetIpInfoAsync(string ipAddress)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_options.BaseUrl}/{ipAddress}?token={_options.Token}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IpInfo>(content);
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
