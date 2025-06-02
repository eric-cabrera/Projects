namespace Assurity.AgentPortal.Contracts.Directus.PageProtectionResponse;

using Newtonsoft.Json;

public class PageProtectionResponseAgentCenterPage
{
    [JsonProperty("protected")]
    public bool Protected { get; set; } = false;
}
