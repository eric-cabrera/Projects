using Assurity.AgentPortal.Contracts.SubaccountUsers;
using System.Text.Json.Serialization;

namespace Assurity.AgentPortal.Accessors.SubAccount.DTOs;

public class PendingSubaccount
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("agentId")]
    public string? AgentId { get; set; }

    [JsonPropertyName("parentUsername")]
    public string? ParentUsername { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("roles")]
    public List<SubAccountRole>? Roles { get; set; }
}