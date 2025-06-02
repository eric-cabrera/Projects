namespace Assurity.AgentPortal.Accessors.ProfilePingDTOs;

using System.Text.Json.Serialization;

public class PingOneResponseDetails
{
    [JsonPropertyName("code")]
    public string Code { set; get; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("innerError")]
    public PingOneResponseDetailsInnerError InnerError { get; set; }
}