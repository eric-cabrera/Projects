namespace Assurity.AgentPortal.Utilities.FeatureManagement;

using Newtonsoft.Json;

public class DirectusFeature
{
    [JsonProperty("feature_name")]
    public string FeatureName { get; set; }

    [JsonProperty("dev")]
    public bool Dev { get; set; }

    [JsonProperty("dev2")]
    public bool Dev2 { get; set; }

    [JsonProperty("test")]
    public bool Test { get; set; }

    [JsonProperty("test2")]
    public bool Test2 { get; set; }

    [JsonProperty("prod")]
    public bool Prod { get; set; }
}
