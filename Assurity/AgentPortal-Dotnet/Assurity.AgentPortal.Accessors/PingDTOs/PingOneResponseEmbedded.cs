namespace Assurity.AgentPortal.Accessors.ProfilePingDTOs;

using System.Text.Json.Serialization;

public class PingOneResponseEmbedded
{
    [JsonPropertyName("devices")]
    public List<PingOneResponseDevice> Devices { get; set; }
}