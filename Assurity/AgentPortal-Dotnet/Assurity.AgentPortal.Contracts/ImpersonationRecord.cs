namespace Assurity.AgentPortal.Contracts;

using System.Text.Json.Serialization;

public class ImpersonationRecord
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("userName")]
    public string? UserName { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("registeredAgentId")]
    public string? RegisteredAgentId { get; set; }

    [JsonPropertyName("agents")]
    public List<AgentRecord>? Agents { get; set; }
}
