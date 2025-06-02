namespace Assurity.AgentPortal.Accessors.SubAccount.DTOs.PingResponses;

using System.Text.Json.Serialization;

public class EnvironmentDTO
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
}
