namespace Assurity.AgentPortal.Contracts.Directus;

using Newtonsoft.Json;

public class DirectusContactsResponse
{
    [JsonProperty("data")]
    public DirectusContractResponseData Data { get; set; } = new DirectusContractResponseData();
}
