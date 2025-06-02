namespace Assurity.AgentPortal.Accessors.DirectusDTOs;

using Newtonsoft.Json;

public class AgentCenterCommissionDatesData
{
    [JsonProperty("agent_center_commission_dates")]
    public List<CommissionDates> AgentCenterCommissionDates { get; set; }
}
