namespace Assurity.AgentPortal.Contracts.AgentContracts
{
    public class PendingRequirementsHierarchyResponse
    {
        public PendingRequirementsHierarchyBranch? Hierarchy { get; set; }

        public ActiveHierarchyFilters? Filters { get; set; }
    }
}
