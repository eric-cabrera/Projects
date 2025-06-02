namespace Assurity.AgentPortal.Contracts.AgentContracts
{
    using Newtonsoft.Json;

    public class AgentHierarchyBranch
    {
        public AgentContractInformation? AgentInformation { get; set; }

        public string? AgentNumber { get; set; }

        public string? CompanyCode { get; set; }

        [JsonProperty("agentLevel")]
        public string? Level { get; set; }

        public string? MarketCode { get; set; }

        public string? Name { get; set; }

        public string? ContractStatus { get; set; }

        public List<Requirement>? PendingRequirements { get; set; }

        public List<AgentHierarchyBranch>? Branches { get; set; }
    }
}
