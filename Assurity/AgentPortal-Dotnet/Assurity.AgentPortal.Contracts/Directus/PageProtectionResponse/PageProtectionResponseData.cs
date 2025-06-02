namespace Assurity.AgentPortal.Contracts.Directus.PageProtectionResponse;

using Newtonsoft.Json;

public class PageProtectionResponseData
{
    [JsonProperty("agent_center_pages")]
    public List<PageProtectionResponseAgentCenterPage> AgentCenterPages { get; set; } = new();
}
