namespace Assurity.PolicyInfo.Contracts.V1
{
    public class AgentHierarchy
    {
        public Agent Agent { get; set; }

        public List<HierarchyAgent> HierarchyAgents { get; set; }
    }
}
