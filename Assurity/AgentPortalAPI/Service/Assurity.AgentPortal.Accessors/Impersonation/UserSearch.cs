namespace Assurity.AgentPortal.Accessors.Impersonation;

using System.Text.Json.Serialization;
using global::MongoDB.Bson;
using global::MongoDB.Bson.Serialization.Attributes;

public class UserSearch
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [JsonPropertyName("PingUserId")]
    public string? PingUserId { get; set; }

    [JsonPropertyName("UserName")]
    public string? UserName { get; set; }

    [JsonPropertyName("Email")]
    public string? Email { get; set; }

    [JsonPropertyName("Name")]
    public string? Name { get; set; }

    [JsonPropertyName("RegisteredAgentId")]
    public string? RegisteredAgentId { get; set; }

    [JsonPropertyName("Agents")]
    public List<Agent> Agents { get; set; }

    [JsonIgnore]
    public DateTime LastSynced { get; set; }
}
