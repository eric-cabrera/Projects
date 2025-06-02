namespace Assurity.AgentPortal.Accessors.SubAccount.DTOs.PingResponses;

using System.Text.Json.Serialization;

public class LinkDTO
{
    [JsonPropertyName("href")]
    public string Href { get; set; } = string.Empty;
}
