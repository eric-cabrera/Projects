namespace Assurity.AgentPortal.Contracts.CommissionsDebt;

public class PolicyDetailFilterValues
{
    public List<string> ViewAsAgentIds { get; set; }

    public List<string> CycleDateFilterValues { get; set; }

    public List<string> PolicyNumberFilterValues { get; set; }

    public List<Agent> WritingAgentFilterValues { get; set; }
}
