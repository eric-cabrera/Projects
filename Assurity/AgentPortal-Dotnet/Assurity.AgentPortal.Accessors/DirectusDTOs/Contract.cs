namespace Assurity.AgentPortal.Accessors.DirectusDTOs
{
    using Newtonsoft.Json;

    public class Contract
    {
        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("agent_level")]
        public string AgentLevel { get; set; }

        [JsonProperty("file")]
        public DirectusFile File { get; set; }
    }
}
