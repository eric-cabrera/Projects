namespace Assurity.AgentPortal.Utilities.FeatureManagement;

using Newtonsoft.Json;

public class DirectusFeatureResponse
{
    [JsonProperty("data")]
    public List<DirectusFeature> Data { get; set; }
}
