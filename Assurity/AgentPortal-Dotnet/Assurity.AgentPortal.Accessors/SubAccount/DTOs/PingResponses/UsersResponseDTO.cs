namespace Assurity.AgentPortal.Accessors.SubAccount.DTOs.PingResponses;

using System.Text.Json.Serialization;

public class UsersResponseDTO
{
    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? UserId { get; set; }

    [JsonPropertyName("_links")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public LinksDTO? Links { get; set; }

    [JsonPropertyName("agentID")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? AgentId { get; set; }

    [JsonPropertyName("environment")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public EnvironmentDTO? Environment { get; set; }

    [JsonPropertyName("account")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public AccountDTO? Account { get; set; }

    [JsonPropertyName("createdAt")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public DateTime? CreatedAt { get; set; }

    [JsonPropertyName("email")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("enabled")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Enabled { get; set; }

    [JsonPropertyName("identityProvider")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IdentityProviderDTO IdentityProvider { get; set; }

    [JsonPropertyName("lifecycle")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public LifecycleDTO? Lifecycle { get; set; }

    [JsonPropertyName("mfaEnabled")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? MfaEnabled { get; set; }

    [JsonPropertyName("population")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public PopulationDTO? Population { get; set; }

    [JsonPropertyName("subaccountData")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public SubAccountDataDTO? SubaccountData { get; set; }

    [JsonPropertyName("updatedAt")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public DateTime UpdatedAt { get; set; }

    [JsonPropertyName("username")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string UserName { get; set; }

    [JsonPropertyName("verifyStatus")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string VerifyStatus { get; set; }
}
