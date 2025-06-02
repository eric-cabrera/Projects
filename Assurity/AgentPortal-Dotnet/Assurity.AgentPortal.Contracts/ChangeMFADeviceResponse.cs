namespace Assurity.AgentPortal.Contracts
{
    using System.Text.Json.Serialization;

    public class ChangeMFADeviceResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("deviceId")]
        public string DeviceId { get; set; } = string.Empty;
    }
}
