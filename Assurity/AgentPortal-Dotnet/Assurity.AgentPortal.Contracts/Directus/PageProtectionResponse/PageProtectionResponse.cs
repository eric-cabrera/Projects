namespace Assurity.AgentPortal.Contracts.Directus.PageProtectionResponse;

using Newtonsoft.Json;

public class PageProtectionResponse
{
    [JsonProperty("data")]
    public PageProtectionResponseData Data { get; set; } = new();
}
