using System;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ctf_sandbox.tests.Drivers.API;

public class APIEmailsDriver : IEmailsDriver
{
    private readonly HttpClient _httpClient;

    public APIEmailsDriver(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task ActivateRegistrationSentTo(string email)
    {
        // Search for messages sent to the email
        var searchResponse = await _httpClient.GetAsync($"search?query=to:{email}");
        searchResponse.EnsureSuccessStatusCode();
        
        var searchResult = await searchResponse.Content.ReadFromJsonAsync<MailpitSearchResult>();
        
        if (searchResult?.Messages == null || searchResult.Messages.Count == 0)
        {
            throw new InvalidOperationException($"No emails found for {email}");
        }
        
        // Find the registration email with subject "Confirm your email"
        var registrationEmail = searchResult.Messages
            .FirstOrDefault(m => m.Subject?.Contains("Confirm your email") == true);
        
        if (registrationEmail == null)
        {
            throw new InvalidOperationException($"No registration email found for {email}");
        }
        
        // Get the full message with HTML content
        var messageResponse = await _httpClient.GetAsync($"message/{registrationEmail.ID}");
        messageResponse.EnsureSuccessStatusCode();
        
        var message = await messageResponse.Content.ReadFromJsonAsync<MailpitMessage>();
        
        if (string.IsNullOrEmpty(message?.Text))
        {
            throw new InvalidOperationException($"No text content found in email for {email}");
        }
        
        // Extract the activation link from "clicking here"
        var activationLink = ExtractActivationLink(message.Text);
        
        if (string.IsNullOrEmpty(activationLink))
        {
            throw new InvalidOperationException($"Could not find activation link in email for {email}");
        }

        // Activate the registration by making a GET request to the link
        var activationResponse = await _httpClient.GetAsync(activationLink);
        var reponse = await activationResponse.Content.ReadAsStringAsync();
        activationResponse.EnsureSuccessStatusCode();
    }
    
    private string? ExtractActivationLink(string textContent)
    {
        // Look for URL in parentheses after "clicking here"
        // Format: "clicking here ( http://... )"
        var regex = new Regex(@"clicking here\s*\(\s*([^\s)]+)\s*\)", RegexOptions.IgnoreCase);
        var match = regex.Match(textContent);
        
        if (match.Success)
        {
            return match.Groups[1].Value;
        }
        
        return null;
    }
}

// DTOs for Mailpit API responses
internal class MailpitSearchResult
{
    public int Total { get; set; }
    public int Count { get; set; }
    public List<MailpitMessageSummary> Messages { get; set; } = new();
}

internal class MailpitMessageSummary
{
    public string ID { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public MailpitAddress From { get; set; } = new();
    public List<MailpitAddress> To { get; set; } = new();
    public DateTime Created { get; set; }
}

internal class MailpitMessage
{
    public string ID { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string HTML { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}

internal class MailpitAddress
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}
