namespace Assurity.AgentPortal.Contracts.AgentContracts
{
    public class AgentHierarchy : AgentHierarchyBranch
    {
        public int TotalActiveAgents { get; set; }

        public int TotalJitAgents { get; set; }

        public int TotalPendingAgents { get; set; }
    }
}
