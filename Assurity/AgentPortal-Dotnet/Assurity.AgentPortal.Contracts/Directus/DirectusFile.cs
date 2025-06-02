namespace Assurity.AgentPortal.Contracts.Directus;

using Newtonsoft.Json;

public class DirectusFile
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;
}