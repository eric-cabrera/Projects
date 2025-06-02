namespace Assurity.AgentPortal.Contracts.Directus;

using Newtonsoft.Json;

public class DirectusRegion
{
    [JsonProperty("display_name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("states")]
    public List<string> States { get; set; } = new();
}
