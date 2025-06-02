namespace Assurity.AgentPortal.Contracts.Directus;

using Newtonsoft.Json;

public class DirectusContactTypeXref
{
    [JsonProperty("agent_center_contact_types_id")]
    public DirectusContractType ContractType { get; set; } = new();
}
