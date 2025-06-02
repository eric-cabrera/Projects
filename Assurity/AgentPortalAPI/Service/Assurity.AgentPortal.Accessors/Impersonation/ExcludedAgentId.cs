namespace Assurity.AgentPortal.Accessors.Impersonation;

using System.Text.Json.Serialization;
using global::MongoDB.Bson;

public class ExcludedAgentId
{
    public ObjectId Id { get; set; }

    [JsonPropertyName("agentId")]
    public string? AgentId { get; set; }
}
