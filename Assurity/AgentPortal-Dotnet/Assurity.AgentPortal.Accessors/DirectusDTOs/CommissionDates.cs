namespace Assurity.AgentPortal.Accessors.DirectusDTOs;

using Newtonsoft.Json;

public class CommissionDates
{
    [JsonProperty("commissions_processed")]
    public List<CommissionDate> CommissionsProcessed { get; set; }

    [JsonProperty("statements_available")]
    public List<CommissionDate> StatementsAvailable { get; set; }

    [JsonProperty("direct_deposit")]
    public List<CommissionDate> DirectDeposit { get; set; }
}
