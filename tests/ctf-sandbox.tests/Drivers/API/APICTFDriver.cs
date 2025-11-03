using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using ctf_sandbox.Areas.CTF.Models;
using ctf_sandbox.Models;
using ctf_sandbox.tests.Dsl;

namespace ctf_sandbox.tests.Drivers.API;

public class APICTFDriver : ICTFDriver
{
    private readonly HttpClient _httpClient;
    private APIEmailsDriver _apiEmailsDriver;
    private string? _jwt;

    public APICTFDriver(HttpClient httpClient,APIEmailsDriver apiEmailsDriver)
    {
        _httpClient = httpClient;
        _apiEmailsDriver = apiEmailsDriver;
        _jwt = null;
    }

    public Task<Emails> CheckEmails()
    {
        return Task.FromResult(new Emails(_apiEmailsDriver));
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

    public async Task CreateTeam(string teamName)
    {
        EnsureAuthenticatedAndSetAuthorizationHeader();

        var response = await _httpClient.PostAsJsonAsync("teams",
            new CreateTeamRequest()
            {
                Name = teamName
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
