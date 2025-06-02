namespace Assurity.AgentPortal.Accessors.SubAccount.DTOs.PingResponses;

using System.Text.Json.Serialization;

public class UserDTO
{
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("emailVerified")]
    public bool EmailVerified { get; set; } = true;

    public NameDTO? Name { get; set; }

    public PopulationDTO? Population { get; set; }

    [JsonPropertyName("password")]
    public PasswordCreationDTO Password { get; set; }

    [JsonPropertyName("agentID")]
    public string AgentID { get; set; }

    [JsonPropertyName("mfaEnabled")]
    public bool MFAEnabled { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("primaryPhone")]
    public string PrimaryPhone { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [JsonPropertyName("subaccountData")]
    public SubAccountDataDTO SubaccountData { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;
}