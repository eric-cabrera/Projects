namespace Assurity.AgentPortal.Contracts.AgentContracts
{
    using Newtonsoft.Json;

    public class PendingRequirementsHierarchyBranch
    {
        public string? AgentNumber { get; set; }

        public string? CompanyCode { get; set; }

        [JsonProperty("agentLevel")]
        public string? Level { get; set; }

        public string? MarketCode { get; set; }

        public string? Name { get; set; }

        public string? ContractStatus { get; set; }

        public string? EmailAddress { get; set; }

        public string? PhoneNumber { get; set; }

        public List<Requirement>? PendingRequirements { get; set; }

        public List<PendingRequirementsHierarchyBranch>? Branches { get; set; }
    }
}
