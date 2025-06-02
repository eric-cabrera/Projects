namespace Assurity.AgentPortal.Contracts.Directus;

using Newtonsoft.Json;

public class DirectusContractType
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
}