namespace Assurity.AgentPortal.Contracts.Directus;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public class DirectusPageMetadataResponse
{
    [JsonProperty("data")]
    public AgentCenterPages Data { get; set; }
}

public class AgentCenterPages
{
    [JsonProperty("agent_center_pages")]
    public List<AgentCenterPage> Pages { get; set; } = new();
}

public class AgentCenterPage
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("slug")]
    public string Slug { get; set; } = string.Empty;

    [JsonProperty("meta_data")]
    public MetadataTag MetaData { get; set; } = new();
}

public class MetadataTag
{
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    [JsonProperty("og_title")]
    public string Og_Title { get; set; } = string.Empty;

    [JsonProperty("og_description")]
    public string Og_Description { get; set; } = string.Empty;
}
