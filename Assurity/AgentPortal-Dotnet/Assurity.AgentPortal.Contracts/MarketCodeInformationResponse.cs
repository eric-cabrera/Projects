namespace Assurity.AgentPortal.Contracts;

using System.Text.Json.Serialization;

public class MarketCodeInformationResponse
{
    [JsonPropertyName("isReverseHirearchyMarketCode")]
    public bool IsReverseHierarchyMarketCode { get; set; }

    [JsonPropertyName("isNewYorkMarketCode")]
    public bool IsNewYorkMarketCode { get; set; }
}
