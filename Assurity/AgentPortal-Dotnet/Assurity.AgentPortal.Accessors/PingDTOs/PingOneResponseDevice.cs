namespace Assurity.AgentPortal.Accessors.ProfilePingDTOs;

using System.Text.Json.Serialization;
using Assurity.AgentPortal.Contracts;

public class PingOneResponseDevice
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("type")]
    public MFAType Type { get; set; }

    [JsonPropertyName("phone")]
    public string Phone { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

}