namespace Assurity.AgentPortal.Contracts.AgentContracts
{
    public class ActiveHierarchyResponse
    {
        public AgentHierarchy? Hierarchy { get; set; }

        public ActiveHierarchyFilters? Filters { get; set; }
    }
}
