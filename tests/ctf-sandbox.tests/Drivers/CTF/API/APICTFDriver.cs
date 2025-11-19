using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using ctf_sandbox.Areas.CTF.Models;
using ctf_sandbox.Models;
using ctf_sandbox.tests.Dsl;

namespace ctf_sandbox.tests.Drivers.CTF.API;

public class APICTFDriver : ICTFDriver
{
    private readonly HttpClient _httpClient;
    private string? _jwt;

    public APICTFDriver(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jwt = null;
    }

    public async Task<bool> CreateAccount(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("account",
            new RegisterAccountRequest()
            {
                Email = email,
                Password = password
            });

        return response.IsSuccessStatusCode;
    }

    public async Task<string?> CreateTeam(string? teamName)
    {
        EnsureAuthenticatedAndSetAuthorizationHeader();

        var response = await _httpClient.PostAsJsonAsync("teams",
            new CreateTeamRequest()
            {
                Name = teamName ?? string.Empty
            });

        if (response.IsSuccessStatusCode)
        {
            return null; // Success, no error
        }

        // Parse error response and extract validation errors
        try
        {
            var problemDetails = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
            var errors = new List<string>();

            if (problemDetails.TryGetProperty("errors", out var errorsProperty))
            {
                foreach (var errorProperty in errorsProperty.EnumerateObject())
                {
                    if (errorProperty.Value.ValueKind == System.Text.Json.JsonValueKind.Array)
                    {
                        foreach (var errorMessage in errorProperty.Value.EnumerateArray())
                        {
                            errors.Add(errorMessage.GetString() ?? string.Empty);
                        }
                    }
                }
            }
            else if (problemDetails.TryGetProperty("message", out var messageProperty))
            {
                errors.Add(messageProperty.GetString() ?? "An error occurred");
            }
            else if (problemDetails.TryGetProperty("title", out var titleProperty))
            {
                errors.Add(titleProperty.GetString() ?? "Validation failed");
            }

            return errors.Any() ? string.Join("; ", errors) : "Validation failed";
        }
        catch
        {
            // Fallback to raw content if JSON parsing fails
            return await response.Content.ReadAsStringAsync();
        }
    }

    public async Task UpdateTeam(string oldTeamName, string newTeamName, string? newDescription = null)
    {
        EnsureAuthenticatedAndSetAuthorizationHeader();

        // First, get the team ID by finding the team with the old name
        var getResponse = await _httpClient.GetAsync("teams");
        getResponse.EnsureSuccessStatusCode();

        var teams = await getResponse.Content.ReadFromJsonAsync<List<Team>>();
        var team = teams?.FirstOrDefault(t => t.Name == oldTeamName);
        
        if (team == null)
        {
            throw new InvalidOperationException($"Team '{oldTeamName}' not found");
        }

        // Update the team
        var response = await _httpClient.PutAsJsonAsync($"teams/{team.Id}",
            new UpdateTeamRequest()
            {
                Name = newTeamName,
                Description = newDescription
            });

        response.EnsureSuccessStatusCode();
    }

    public async Task<IpInfo> GetIpInfo(string ipAddress)
    {
        EnsureAuthenticatedAndSetAuthorizationHeader();

        var response = await _httpClient.GetAsync($"ipinfo/{ipAddress}");
        response.EnsureSuccessStatusCode();

        var ipInfo = await response.Content.ReadFromJsonAsync<IpInfo>();
        return ipInfo ?? throw new InvalidOperationException("Failed to deserialize IP info response");
    }

    public async Task<bool> IsTeamAvailable(string teamName)
    {
        EnsureAuthenticatedAndSetAuthorizationHeader();

        var response = await _httpClient.GetAsync("teams");
        response.EnsureSuccessStatusCode();

        var teams = await response.Content.ReadFromJsonAsync<List<Team>>();
        
        return teams?.Any(t => t.Name == teamName) ?? false;
    }

    public Task<bool> IsUserSignedIn(string email)
    {
        var decodedJwt = new JwtSecurityTokenHandler().ReadJwtToken(_jwt);
        return Task.FromResult(decodedJwt.Claims.Any(c => c.Type == "email" && c.Value == email));
    }

    public async Task SignIn(string email, string password)
    {
        var result = await _httpClient.PostAsJsonAsync("auth",
         new LoginRequest()
         {
             Username = email,
             Password = password
         });

        result.EnsureSuccessStatusCode();
        _jwt = await result.Content.ReadAsStringAsync();
    }

    private void EnsureAuthenticatedAndSetAuthorizationHeader()
    {
        if (string.IsNullOrEmpty(_jwt))
        {
            throw new InvalidOperationException("User must be signed in to perform this action");
        }

        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwt);
    }
}
