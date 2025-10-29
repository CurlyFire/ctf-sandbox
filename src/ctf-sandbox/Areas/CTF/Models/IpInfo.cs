using System.Text.Json.Serialization;

namespace ctf_sandbox.Areas.CTF.Models;

public class IpInfo
{
    [JsonPropertyName("ip")]
    public string Ip { get; set; }

    [JsonPropertyName("hostname")]
    public string? Hostname { get; set; }

    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("region")]
    public string? Region { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("loc")]
    public string? Location { get; set; }

    [JsonPropertyName("org")]
    public string? Organization { get; set; }

    [JsonPropertyName("postal")]
    public string? PostalCode { get; set; }

    [JsonPropertyName("timezone")]
    public string? Timezone { get; set; }

    [JsonPropertyName("anycast")]
    public bool? Anycast { get; set; }
}
