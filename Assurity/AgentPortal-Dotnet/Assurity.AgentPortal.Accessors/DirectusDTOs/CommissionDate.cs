namespace Assurity.AgentPortal.Accessors.DirectusDTOs;

using Newtonsoft.Json;

public class CommissionDate
{
    [JsonProperty("Date")]
    public string Date { get; set; }
}
