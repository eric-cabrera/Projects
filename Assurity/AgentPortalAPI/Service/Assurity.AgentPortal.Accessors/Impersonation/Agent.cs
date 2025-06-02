namespace Assurity.AgentPortal.Accessors.Impersonation;

using System.Text.Json.Serialization;

public class Agent
{
    [JsonPropertyName("AgentId")]
    public string AgentId { get; set; }

    [JsonPropertyName("Name")]
    public string Name { get; set; }
}
