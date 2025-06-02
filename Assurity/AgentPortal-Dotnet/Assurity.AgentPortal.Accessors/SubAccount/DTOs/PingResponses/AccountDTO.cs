namespace Assurity.AgentPortal.Accessors.SubAccount.DTOs.PingResponses;

using System.Text.Json.Serialization;

public class AccountDTO
{
    [JsonPropertyName("canAuthenticate")]
    public bool CanAuthenticate { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}
