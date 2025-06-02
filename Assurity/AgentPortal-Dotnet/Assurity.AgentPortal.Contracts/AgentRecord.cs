namespace Assurity.AgentPortal.Contracts;

using System.Text.Json.Serialization;

public class AgentRecord
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("agentIds")]
    public List<string> AgentIds { get; set; }
}
