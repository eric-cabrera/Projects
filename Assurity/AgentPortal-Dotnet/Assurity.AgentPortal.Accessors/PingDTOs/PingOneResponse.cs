namespace Assurity.AgentPortal.Accessors.ProfilePingDTOs;

using System.Text.Json.Serialization;

public class PingOneResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("details")]
    public List<PingOneResponseDetails> Details { get; set; }

    [JsonPropertyName("_embedded")]
    public PingOneResponseEmbedded Embedded { get; set; }
}