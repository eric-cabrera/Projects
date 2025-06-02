namespace Assurity.AgentPortal.Contracts.CommissionsDebt;

public class WritingAgentFilters
{
    public List<string> ViewAsAgentIds { get; set; }

    public List<string> CycleDateFilterValues { get; set; }

    public List<Agent> WritingAgentFilterValues { get; set; }
}
