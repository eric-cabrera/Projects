namespace Assurity.AgentPortal.Contracts
{
    using System.Text.Json.Serialization;

    public class ChangePasswordResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }
}
