namespace Assurity.AgentPortal.Contracts;

using System.Text.Json.Serialization;

public class AdditionalAgentIds
{
    [JsonPropertyName("agentIds")]
    public List<string> AgentIds { get; set; }
}
