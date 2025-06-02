namespace Assurity.AgentPortal.Accessors.SubAccount.DTOs.PingResponses;

using System.Text.Json.Serialization;
using Assurity.AgentPortal.Contracts.SubaccountUsers;

public class SubAccountDataDTO
{
    [JsonPropertyName("parentUsername")]
    public string? ParentUsername { get; set; } = string.Empty;

    [JsonPropertyName("parentAgentId")]
    public string? ParentAgentId { get; set; } = string.Empty;

    [JsonPropertyName("roles")]
    public List<SubAccountRole> Roles { get; set; } = new List<SubAccountRole>();
}