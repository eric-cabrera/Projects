namespace Assurity.AgentPortal.Contracts.CommissionsDebt;

public class DebtFilterValues
{
    public List<string> ViewAsAgentIds { get; set; }

    public List<Hierarchy> HierarchyFilterValues { get; set; }

    public List<string> PolicyNumberFilterValues { get; set; }

    public List<string> StatusFilterValues { get; set; }

    public List<Agent> AgentFilterValues { get; set; }
}
