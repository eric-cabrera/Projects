namespace Assurity.AgentPortal.Accessors.SubAccount.DTOs.PingResponses;

using System.Text.Json.Serialization;

public class PasswordCreationDTO
{
    [JsonPropertyName("value")]
    public string Value { get; set; }

    [JsonPropertyName("forceChange")]
    public bool ForceChange { get; set; }
}
