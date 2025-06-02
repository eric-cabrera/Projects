namespace Assurity.AgentPortal.Contracts.Directus;

using Newtonsoft.Json;

public class DirectusContractResponseData
{
    [JsonProperty("agent_center_contacts")]
    public List<DirectusContact> Contacts { get; set; } = new List<DirectusContact>();
}