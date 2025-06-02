namespace Assurity.AgentPortal.Contracts
{
    using System.Text.Json.Serialization;

    public class ConfirmMFADeviceResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("attemptsLeft")]
        public int? AttemptsLeft { get; set; }
    }
}
