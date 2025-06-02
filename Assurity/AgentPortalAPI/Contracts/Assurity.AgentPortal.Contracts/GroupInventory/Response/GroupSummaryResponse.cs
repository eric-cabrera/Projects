namespace Assurity.AgentPortal.Contracts.GroupInventory.Response;

public class GroupSummaryResponse
{
    public List<GroupSummary>? GroupSummaries { get; set; }

    public int TotalSummaries { get; set; }

    public GroupSummaryFilters? Filters { get; set; }
}
