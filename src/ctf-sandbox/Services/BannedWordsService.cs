using System.Text.Json;
using Microsoft.Extensions.Options;

namespace ctf_sandbox.Services;

public class BannedWordsService : IBannedWordsService
{
    private readonly HttpClient _httpClient;
    private readonly BannedWordsOptions _options;
    private readonly ILogger<BannedWordsService> _logger;

    public BannedWordsService(
        HttpClient httpClient, 
        IOptions<BannedWordsOptions> options,
        ILogger<BannedWordsService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<bool> ContainsBannedWordAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        try
        {
            var response = await _httpClient.GetAsync($"{_options.BaseUrl}/BannedWords");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            var bannedWords = JsonSerializer.Deserialize<string[]>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (bannedWords == null || bannedWords.Length == 0)
            {
                return false;
            }

            var lowerText = text.ToLowerInvariant();
            return bannedWords.Any(word => lowerText.Contains(word.ToLowerInvariant()));
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to retrieve banned words from API");
            return false;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse banned words response");
            return false;
        }
    }
}
