namespace Assurity.AgentPortal.Accessors.DirectusDTOs
{
    using Newtonsoft.Json;

    public class ContractsQueryData
    {
        [JsonProperty("agent_center_contracts")]
        public List<Contract> Contracts { get; set; }
    }
}
