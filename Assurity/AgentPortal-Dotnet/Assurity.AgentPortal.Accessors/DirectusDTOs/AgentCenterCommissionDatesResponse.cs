namespace Assurity.AgentPortal.Accessors.DirectusDTOs;

using Newtonsoft.Json;

public class AgentCenterCommissionDatesResponse
{
    [JsonProperty("data")]
    public AgentCenterCommissionDatesData Data { get; set; }
}
