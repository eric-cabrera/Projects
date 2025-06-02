namespace Assurity.AgentPortal.Contracts.SubaccountUsers;

using System.Text.Json.Serialization;

public class SubaccountData
{
    [JsonPropertyName("parentUserName")]
    public string? ParentUserName { get; set; } = string.Empty;

    [JsonPropertyName("parentAgentId")]
    public string? ParentAgentId { get; set; } = string.Empty;

    [JsonPropertyName("roles")]
    public List<SubAccountRole> Roles { get; set; } = new List<SubAccountRole>();
}
