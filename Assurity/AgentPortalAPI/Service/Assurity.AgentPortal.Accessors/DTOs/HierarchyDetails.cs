namespace Assurity.AgentPortal.Accessors.DTOs
{
    public class HierarchyDetails
    {
        public int TotalActiveAgents { get; set; }

        public int TotalJitAgents { get; set; }

        public int TotalPendingAgents { get; set; }

        public string? AgentNumber { get; set; }

        public string? CompanyCode { get; set; }

        public string? Level { get; set; }

        public string? MarketCode { get; set; }

        public string? Name { get; set; }

        public string? ContractStatus { get; set; }

        public string? ContractType { get; set; }

        public List<object>? PendingRequirements { get; set; }

        public List<HierarchyBranch> Branches { get; set; }
    }
}